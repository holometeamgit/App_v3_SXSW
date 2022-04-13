using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// DeepLink Stadium Costructor
/// </summary>
public class DeepLinkStadiumConstructor : MonoBehaviour {
    [SerializeField]
    private DeepLinkRoomPopup _deepLinkRoomPopup;
    [SerializeField]
    private DeepLinkChecker _popupShowChecker;
    [SerializeField]
    private Color _highlightMSGColor;

    public static Action<StreamJsonData.Data> OnShow;
    public static Action OnBroadcastFinished = delegate { };
    public static Action<WebRequestError> OnShowError;
    public static Action OnHide;

    private StreamJsonData.Data _data;

    private CancellationTokenSource cancelTokenSource;
    private CancellationToken cancellationToken;
    private const int DELAY = 5000;

    private void OnEnable() {
        OnShow += Show;
        OnShowError += ShowError;
        OnHide += Hide;
        OnBroadcastFinished += ShowNoLongerLive;
    }

    private void OnDisable() {
        OnShow -= Show;
        OnShowError -= ShowError;
        OnHide -= Hide;
        OnBroadcastFinished -= ShowNoLongerLive;
    }

    private void Show(StreamJsonData.Data data) {
        OnReceivedData(data, ShowData);
    }

    private void OnReceivedData(StreamJsonData.Data data, Action<StreamJsonData.Data> onSuccessTask) {
        _popupShowChecker.OnReceivedData(data, onSuccessTask);
    }

    private async void ShowData(StreamJsonData.Data data) {

        cancelTokenSource = new CancellationTokenSource();
        cancellationToken = cancelTokenSource.Token;

        if (data.status == StreamJsonData.Data.LIVE_ROOM_STR) {
            ShowOnline(data);
        } else {
            ShowOffline(data);
        }

        try {
            await Task.Delay(DELAY);
            if (!cancellationToken.IsCancellationRequested) {
                StreamCallBacks.onReceiveStadiumLink?.Invoke(data.user);
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
            bool isRoom = false;
            DeepLinkStreamData deepLinkRoomData = new DeepLinkStreamData(isRoom, title, description, _data.id.ToString(), _data.user, _data.agora_channel, online, closeBtn, shareBtn);
            _deepLinkRoomPopup.Show(deepLinkRoomData);
            _data = null;
        }
    }

    private void ShowOnline(StreamJsonData.Data data) {
        string title = ConvertApostropheSmartToStraight($"<color=#{ColorUtility.ToHtmlStringRGBA(_highlightMSGColor)}>{data.user}</color>'s stadium is online");
        string description = string.Empty;
        bool online = true;
        bool closeBtn = false;
        bool shareBtn = false;
        bool isRoom = false;
        DeepLinkStreamData deepLinkRoomData = new DeepLinkStreamData(isRoom, title, description, data.id.ToString(), data.user, data.agora_channel, online, closeBtn, shareBtn);
        _deepLinkRoomPopup.Show(deepLinkRoomData);
        _data = data;
    }

    private void ShowOffline(StreamJsonData.Data data) {
        string title = ConvertApostropheSmartToStraight($"<color=#{ColorUtility.ToHtmlStringRGBA(_highlightMSGColor)}>{data.user}</color>'s stadium is currently offline");
        string description = string.Empty;
        bool online = false;
        bool closeBtn = false;
        bool shareBtn = false;
        bool isRoom = false;
        DeepLinkStreamData deepLinkRoomData = new DeepLinkStreamData(isRoom, title, description, data.id.ToString(), data.user, data.agora_channel, online, closeBtn, shareBtn);
        _deepLinkRoomPopup.Show(deepLinkRoomData);
        _data = null;
    }

    private string ConvertApostropheSmartToStraight(string value) {
        if (string.IsNullOrEmpty(value)) {
            return "";
        }
        return value.Replace("’", "'");
    }


    private void ShowError(WebRequestError webRequestError) {
        _popupShowChecker.OnReceivedData(() => WarningConstructor.ActivateSingleButton("This user doesn't exist", "Please make sure that the link you received is correct.", "Ok"));
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
