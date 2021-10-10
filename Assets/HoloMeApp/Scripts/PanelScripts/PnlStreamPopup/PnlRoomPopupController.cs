using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using System;


//добавить авторефреш после получения ссылки. он прерывается только если пользователь нажимает отмену на интерфейсе.
//отдельный класс который следит открыта домашняястраница или нет.
//если пользователь кликает вне попапа, то запрещено это делать.. наверное

public class PnlRoomPopupController {
    private RoomPopupShowChecker _roomPopupShowChecker;

    private RoomJsonData _receivedRoomJsonData;
    private RoomJsonData _startedRoomJsonData;
    private CancellationTokenSource _cancellationTokenSource;
    private CancellationToken _cancellationToken;

    private bool _isWaitingOpen;
    private const int  CHECK_COOLDOWN = 1000;

    public PnlRoomPopupController(RoomPopupShowChecker roomPopupShowChecker) {
        Construct(roomPopupShowChecker);
    }

    public void Construct(RoomPopupShowChecker roomPopupShowChecker) {
        _roomPopupShowChecker = roomPopupShowChecker;
        StreamCallBacks.onRoomDataReceived += OnReceivedRoomData;
        StreamCallBacks.onRoomClosed += OnRoomStreamClosed;
        StreamCallBacks.onOpenRoom += OnOpenRoom;
        StreamCallBacks.onShareRoom += OnShareRoom;
    }

    public void Test() {
        RoomJsonData roomJsonData = new RoomJsonData();
        roomJsonData.agora_channel = "IVKLIM1";
        OnReceivedRoomData(roomJsonData);

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task.Delay(CHECK_COOLDOWN * 3).ContinueWith(task => {
            Debug.Log("Request cancel ");
            _cancellationTokenSource.Cancel(true);

        }, taskScheduler);
    }

    #region from app
    private void OnReceivedRoomData(RoomJsonData roomJsonData) {
        if (_cancellationTokenSource != null) {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        } else {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        WaitForCanShow().ContinueWith((task) => {

            HelperFunctions.DevLog("IsCanceled " + task.IsCanceled + " IsCompleted " + task.IsCompleted);
            //on canceled
            if (task.IsCanceled) {
                HelperFunctions.DevLog("Previouses room deeplink request was interrupted");
                // on complete
            } else if (task.IsCompleted) {
                HelperFunctions.DevLog("Room popup request. Room: " + roomJsonData.agora_channel);
                _receivedRoomJsonData = roomJsonData;

                if (_receivedRoomJsonData.status == StreamJsonData.Data.LIVE_ROOM_STR) {
                    StreamCallBacks.onShowPopUpRoomOnline(_receivedRoomJsonData.user);
                    //TODO щё подписать на количество пользователей в комнате
                } else {
                    StreamCallBacks.onShowPopUpRoomOffline(_receivedRoomJsonData.user);
                }
            }
        }, taskScheduler);

        //если пришло другая комната, то текущую скрыть и открыть другую! мб если понадобится
        //если стрим закончился и нет новых входящих линков, то показывать что пользователь больше не лайф
    }

    private void OnRoomStreamClosed() {
        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        WaitForCanShow().ContinueWith((task) => {
            if (_startedRoomJsonData.user != _receivedRoomJsonData.user)
                return;
            StreamCallBacks.onShowPopUpRoomEnded(_receivedRoomJsonData.user);
        }, taskScheduler);
    }
    #endregion

    #region from UI
    private void OnOpenRoom() {
        _startedRoomJsonData = _receivedRoomJsonData;
    }

    private void OnShareRoom() {
        StreamCallBacks.onGetRoomLink?.Invoke(_receivedRoomJsonData.user);
    }
    #endregion

    private async Task WaitForCanShow() {
        _isWaitingOpen = true;
        while (!_roomPopupShowChecker.CanShow()) {
            if (_cancellationToken.IsCancellationRequested) {
                _isWaitingOpen = false;
                _cancellationToken.ThrowIfCancellationRequested();
            }
            await Task.Delay(CHECK_COOLDOWN);
            Debug.Log("Check room ");
        }
        _isWaitingOpen = false;
    }

    ~PnlRoomPopupController() {
        StreamCallBacks.onRoomDataReceived -= OnReceivedRoomData;
        StreamCallBacks.onRoomClosed -= OnRoomStreamClosed;
        StreamCallBacks.onOpenRoom -= OnOpenRoom;
        StreamCallBacks.onShareRoom -= OnShareRoom;

        if (_cancellationTokenSource != null) {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}
