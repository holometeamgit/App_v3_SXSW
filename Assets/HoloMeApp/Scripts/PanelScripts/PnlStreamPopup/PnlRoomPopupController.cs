using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using System;

public class PnlRoomPopupController {
    private RoomPopupShowChecker _roomPopupShowChecker;
    private StreamerCountUpdater _streamerCountUpdater;

    private RoomJsonData _receivedRoomJsonData;
    private RoomJsonData _startedRoomJsonData;
    private CancellationTokenSource _showCancellationTokenSource;
    private CancellationToken _showCancellationToken;

    private bool _isWaitingOpen;
    private const int CHECK_COOLDOWN = 1000;

    private bool _isShow;

    public PnlRoomPopupController(RoomPopupShowChecker roomPopupShowChecker, StreamerCountUpdater streamerCountUpdater) {
        Construct(roomPopupShowChecker, streamerCountUpdater);
    }

    public void Construct(RoomPopupShowChecker roomPopupShowChecker, StreamerCountUpdater streamerCountUpdater) {
        _roomPopupShowChecker = roomPopupShowChecker;
        _streamerCountUpdater = streamerCountUpdater;

        _streamerCountUpdater.OnCountUpdated += UpdateUserCount;

        //from app
        StreamCallBacks.onRoomDataReceived += OnReceivedRoomData;
        StreamCallBacks.onRoomClosed += OnRoomStreamClosed;

        //from ui
        StreamCallBacks.onOpenRoom += OnOpenRoom;
        StreamCallBacks.onShareRoom += OnShareRoom;
        StreamCallBacks.onPopUpClosed += OnPopUpClosed;
        StreamCallBacks.onPopUpStartOpen += OnPopUpStartOpen;
    }

    #region from app
    private void OnReceivedRoomData(RoomJsonData roomJsonData) {
        if (_showCancellationTokenSource != null) {
            _showCancellationTokenSource.Cancel();
            _showCancellationTokenSource.Dispose();

            _showCancellationTokenSource = new CancellationTokenSource();
            _showCancellationToken = _showCancellationTokenSource.Token;
        } else {
            _showCancellationTokenSource = new CancellationTokenSource();
            _showCancellationToken = _showCancellationTokenSource.Token;
        }

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        WaitForCanShow().ContinueWith((task) => {
            //on canceled
            if (task.IsCanceled) {
                HelperFunctions.DevLog("Previouses room deeplink request was interrupted");
                // on complete
            } else if (task.IsCompleted) {
                HelperFunctions.DevLog("Room popup request. Room: " + roomJsonData.agora_channel);
                _receivedRoomJsonData = roomJsonData;

                if (_receivedRoomJsonData.status == StreamJsonData.Data.LIVE_ROOM_STR) {
                    StreamCallBacks.onShowPopUpRoomOnline(_receivedRoomJsonData.user);
                    _streamerCountUpdater.StartCheck(_receivedRoomJsonData.agora_channel);
                } else {
                    StreamCallBacks.onShowPopUpRoomOffline(_receivedRoomJsonData.user);
                }

                WaitIfNeedHide().Start();
            }
        }, taskScheduler);
    }

    private void OnRoomStreamClosed() {
        //don't need invoke _cancellationTokenSource
        if (_isWaitingOpen)
            return;

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        WaitForCanShow().ContinueWith((task) => {
            if (_startedRoomJsonData.user != _receivedRoomJsonData.user)
                return;
            StreamCallBacks.onShowPopUpRoomEnded(_receivedRoomJsonData.user);
            WaitIfNeedHide().Start();
        }, taskScheduler);
    }

    private void UpdateUserCount(int count) {
        StreamCallBacks.onUpdateUserCount?.Invoke(count);
    }
    #endregion

    #region from UI
    private void OnOpenRoom() {
        _startedRoomJsonData = _receivedRoomJsonData;
        StreamCallBacks.onPlayLiveStream?.Invoke(_startedRoomJsonData.user, _startedRoomJsonData.agora_channel, _startedRoomJsonData.id, true);
    }

    private void OnShareRoom() {
        StreamCallBacks.onGetRoomLink?.Invoke(_receivedRoomJsonData.user);
    }

    private void OnPopUpClosed() {
        _streamerCountUpdater.StopCheck();
        _isShow = false;
    }

    private void OnPopUpStartOpen() {
        _isShow = true;
    }
    #endregion

    private async Task WaitForCanShow() {
        _isWaitingOpen = true;
        while (!_roomPopupShowChecker.CanShow()) {
            if (_showCancellationToken.IsCancellationRequested) {
                _isWaitingOpen = false;
                _showCancellationToken.ThrowIfCancellationRequested();
            }
            await Task.Delay(CHECK_COOLDOWN);
        }
        _isWaitingOpen = false;
    }

    private async Task WaitIfNeedHide() {
        while (_roomPopupShowChecker.CanShow() && _isShow) {
            await Task.Delay(CHECK_COOLDOWN);
            Debug.Log("WaitIfNeedHide");
        }

        StreamCallBacks.onClosePopUp?.Invoke();
    }

    ~PnlRoomPopupController() {
        _streamerCountUpdater.OnCountUpdated -= UpdateUserCount;

        //from app
        StreamCallBacks.onRoomDataReceived -= OnReceivedRoomData;
        StreamCallBacks.onRoomClosed -= OnRoomStreamClosed;

        //from ui
        StreamCallBacks.onOpenRoom -= OnOpenRoom;
        StreamCallBacks.onShareRoom -= OnShareRoom;
        StreamCallBacks.onPopUpClosed -= OnPopUpClosed;
        StreamCallBacks.onPopUpStartOpen -= OnPopUpStartOpen;

        if (_showCancellationTokenSource != null) {
            _showCancellationTokenSource.Cancel();
            _showCancellationTokenSource.Dispose();
        }


    }
}
