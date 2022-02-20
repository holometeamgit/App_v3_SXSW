using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WindowManager.Extenject;

/// <summary>
/// Deep Link Room Constructor
/// </summary>
public class DeepLinkRoomConstructor : MonoBehaviour {

    [SerializeField]
    private DeepLinkRoomPopup _deepLinkRoomPopup;
    [SerializeField]
    private DeepLinkChecker _popupShowChecker;
    [SerializeField]
    private Color _highlightMSGColor;

    public static Action<RoomJsonData> OnShow;
    public static Action<WebRequestError> OnShowError;
    public static Action OnHide;

    private RoomJsonData _data;

    private CancellationTokenSource cancelTokenSource;
    private CancellationToken cancellationToken;
    private const int DELAY = 5000;

    private void OnEnable() {
        OnShow += Show;
        OnShowError += ShowError;
        OnHide += Hide;
        StreamCallBacks.onRoomBroadcastFinished += ShowNoLongerLive;
    }

    private void OnDisable() {
        OnShow -= Show;
        OnShowError -= ShowError;
        OnHide -= Hide;
        StreamCallBacks.onRoomBroadcastFinished -= ShowNoLongerLive;
    }

    private void Show(RoomJsonData data) {
        cancelTokenSource = new CancellationTokenSource();
        cancellationToken = cancelTokenSource.Token;
        OnReceivedData(data, ShowData);
    }

    private void OnReceivedData(RoomJsonData data, Action<RoomJsonData> onSuccessTask) {
        _popupShowChecker.OnReceivedData(data, onSuccessTask);
    }

    private async void ShowData(RoomJsonData data) {
        if (data.status == StreamJsonData.Data.LIVE_ROOM_STR) {
            ShowOnline(data);
        } else {
            ShowOffline(data);
        }

        try {
            await Task.Delay(DELAY);
            if (!cancellationToken.IsCancellationRequested) {
                StreamCallBacks.onReceiveRoomLink?.Invoke(data.user);
            }
        } finally {
            if (cancelTokenSource != null) {
                cancelTokenSource.Dispose();
                cancelTokenSource = null;
            }
        }
    }

    private void ShowNoLongerLive() {
        if (_data != null) {
            string title = $"<color=#{ColorUtility.ToHtmlStringRGBA(_highlightMSGColor)}>{_data.user}</color> is no longer live";
            string description = string.Empty;
            bool online = false;
            bool closeBtn = true;
            bool shareBtn = false;
            DeepLinkRoomData deepLinkRoomData = new DeepLinkRoomData(title, description, _data, online, closeBtn, shareBtn);
            _deepLinkRoomPopup.Show(deepLinkRoomData);
            _data = null;
        }
    }

    private void ShowOnline(RoomJsonData data) {
        string title = $"<color=#{ColorUtility.ToHtmlStringRGBA(_highlightMSGColor)}>{data.user}</color>’s room is online";
        string description = string.Empty;
        bool online = true;
        bool closeBtn = false;
        bool shareBtn = false;
        DeepLinkRoomData deepLinkRoomData = new DeepLinkRoomData(title, description, data, online, closeBtn, shareBtn);
        _deepLinkRoomPopup.Show(deepLinkRoomData);
        _data = data;
    }

    private void ShowOffline(RoomJsonData data) {
        string title = $"<color=#{ ColorUtility.ToHtmlStringRGBA(_highlightMSGColor)}>{data.user}</color>’s room is currently offline";
        string description = string.Empty;
        bool online = false;
        bool closeBtn = false;
        bool shareBtn = true;
        DeepLinkRoomData deepLinkRoomData = new DeepLinkRoomData(title, description, data, online, closeBtn, shareBtn);
        _deepLinkRoomPopup.Show(deepLinkRoomData);
        _data = null;
    }

    private void ShowError(WebRequestError webRequestError) {
        GeneralPopUpData.ButtonData closeButton = new GeneralPopUpData.ButtonData("Ok", null);
        GeneralPopUpData data = new GeneralPopUpData("This user doesn't exist", "Please make sure that the link you received is correct.", closeButton);
        _popupShowChecker.OnReceivedData(() => WarningConstructor.OnShow?.Invoke(data));
    }

    private void Cancel() {
        if (cancelTokenSource != null) {
            cancelTokenSource.Cancel();
            cancelTokenSource = null;
        }
    }

    private void Hide() {
        _deepLinkRoomPopup.Hide();
        Cancel();
        _popupShowChecker.Cancel();
    }
}
