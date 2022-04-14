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

        if (data.Status == StreamJsonData.Data.LIVE_ROOM_STR || data.Status == StreamJsonData.Data.LIVE_STR) {
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
            DeepLinkStreamData deepLinkStreamData = new DeepLinkStreamData(DeepLinkStreamData.DeepLinkPopup.NoLongerLive, _data);
            _deepLinkRoomPopup.Show(deepLinkStreamData);
            _data = null;
        }
    }

    private void ShowOnline(IData data) {
        DeepLinkStreamData deepLinkStreamData = new DeepLinkStreamData(DeepLinkStreamData.DeepLinkPopup.Online, data);
        _deepLinkRoomPopup.Show(deepLinkStreamData);
        _data = data;
    }

    private void ShowOffline(IData data) {
        DeepLinkStreamData deepLinkStreamData = new DeepLinkStreamData(DeepLinkStreamData.DeepLinkPopup.Offline, data);
        _deepLinkRoomPopup.Show(deepLinkStreamData);
        _data = null;
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
