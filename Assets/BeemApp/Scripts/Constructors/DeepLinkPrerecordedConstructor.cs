using Beem.Permissions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DeepLink Stream Costructor
/// </summary>
public class DeepLinkPrerecordedConstructor : MonoBehaviour {
    [SerializeField]
    private DeepLinkChecker _popupShowChecker;

    private PermissionController _permissionController = new PermissionController();

    public static Action<StreamJsonData.Data> OnShow = delegate { };
    public static Action<WebRequestError> OnShowError = delegate { };
    public static Action OnHide = delegate { };
    private void OnEnable() {
        OnShow += Show;
        OnShowError += ShowError;
    }

    private void OnDisable() {
        OnShow -= Show;
        OnShowError -= ShowError;
    }

    private void Show(StreamJsonData.Data data) {
        OnReceivedARMessageData(data, ActivateData);
    }

    private void OnReceivedARMessageData(StreamJsonData.Data data, Action<StreamJsonData.Data> onSuccessTask) {
        _popupShowChecker.OnReceivedData(data, onSuccessTask);
    }

    private void ActivateData(StreamJsonData.Data data) {
        _permissionController.CheckCameraMicAccess(() => {
            MenuConstructor.OnActivated?.Invoke(false);
            ARMsgRecordConstructor.OnActivated?.Invoke(false);
            StreamOverlayConstructor.onDeactivatedAsBroadcaster?.Invoke();
            ARenaConstructor.onActivateForPreRecorded?.Invoke(data, false);
            PrerecordedVideoConstructor.OnActivated?.Invoke(data);
            PnlRecord.CurrentUser = data.user;
        });
    }

    private void ShowError(WebRequestError webRequestError) {
        _popupShowChecker.OnReceivedData(() => WarningConstructor.ActivateSingleButton("This video doesn't exist", "Please make sure that the link you received is correct.", "Ok"));
    }
}
