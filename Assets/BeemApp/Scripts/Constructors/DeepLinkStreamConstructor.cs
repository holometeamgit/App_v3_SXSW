using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

/// <summary>
/// Deep Link Stream Constructor
/// </summary>
public class DeepLinkStreamConstructor : MonoBehaviour {

    [SerializeField]
    private VideoUploader _videoUploader;

    [SerializeField]
    private DeepLinkPopup _deepLinkRoomPopup;
    [SerializeField]
    private DeepLinkChecker _popupShowChecker;
    [SerializeField]
    private Color _highlightMSGColor;

    public static Action<IData> OnShow;
    public static Action OnBroadcastFinished = delegate { };
    public static Action<WebRequestError> OnShowError;
    public static Action OnHide;

    private IData _data;

    private GetRoomController _getRoomController;
    private GetStadiumController _getStadiumController;

    private CancellationTokenSource cancelTokenSource;
    private CancellationToken cancellationToken;
    private const int DELAY = 5000;

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler) {
        _getRoomController = new GetRoomController(_videoUploader, webRequestHandler);
        _getStadiumController = new GetStadiumController(_videoUploader, webRequestHandler);
    }

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

    private void Show(IData data) {
        OnReceivedData(data, ShowData);
    }

    private void OnReceivedData(IData data, Action<IData> onSuccessTask) {
        _popupShowChecker.OnReceivedData(data, onSuccessTask);
    }

    private async void ShowData(IData data) {

        cancelTokenSource = new CancellationTokenSource();
        cancellationToken = cancelTokenSource.Token;

        if (data.Status == StreamJsonData.Data.LIVE_ROOM_STR) {
            ShowOnline(data);
        } else {
            ShowOffline(data);
        }

        try {
            await Task.Delay(DELAY);
            if (!cancellationToken.IsCancellationRequested) {
                if (data is RoomJsonData) {
                    _getRoomController.GetRoomByUsername(data.Username, Show, ShowError);
                } else {
                    _getStadiumController.GetStadiumByUsername(data.Username, Show, ShowError);
                }
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
            string title = $"<color=#{ColorUtility.ToHtmlStringRGBA(_highlightMSGColor)}>{_data.Username}</color> is no longer live";
            string description = string.Empty;
            bool online = false;
            bool closeBtn = true;
            bool shareBtn = false;
            DeepLinkStreamData deepLinkRoomData = new DeepLinkStreamData(title, description, _data, online, closeBtn, shareBtn);
            _deepLinkRoomPopup.Show(deepLinkRoomData);
            _data = null;
        }
    }

    private void ShowOnline(IData data) {
        string title = ConvertApostropheSmartToStraight($"<color=#{ColorUtility.ToHtmlStringRGBA(_highlightMSGColor)}>{data.Username}</color>'s is now online");
        string description = string.Empty;
        bool online = true;
        bool closeBtn = false;
        bool shareBtn = false;
        DeepLinkStreamData deepLinkRoomData = new DeepLinkStreamData(title, description, data, online, closeBtn, shareBtn);
        _deepLinkRoomPopup.Show(deepLinkRoomData);
        _data = data;
    }

    private void ShowOffline(IData data) {
        string title = ConvertApostropheSmartToStraight($"<color=#{ColorUtility.ToHtmlStringRGBA(_highlightMSGColor)}>{data.Username}</color>'s is currently offline");
        string description = "This page will refresh automatically\nwhen they go live";
        bool online = false;
        bool closeBtn = false;
        bool shareBtn = (data is RoomJsonData);
        DeepLinkStreamData deepLinkRoomData = new DeepLinkStreamData(title, description, data, online, closeBtn, shareBtn);
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
