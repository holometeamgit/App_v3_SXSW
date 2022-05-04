using Beem.Permissions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Constructor for opening deep Link video
/// </summary>
public class DeepLinkVideoConstructor : MonoBehaviour {

    [SerializeField]
    private DeepLinkChecker _popupShowChecker;

    private PermissionController _permissionController = new PermissionController();

    public static Action<IData> OnShow = delegate { };
    public static Action<WebRequestError> OnShowError = delegate { };

    private void OnEnable() {
        OnShow += Show;
        OnShowError += ShowError;
    }

    private void OnDisable() {
        OnShow -= Show;
        OnShowError -= ShowError;
    }

    private void Show(IData data) {
        OnReceivedARMessageData(data, ActivateData);
    }

    private void OnReceivedARMessageData(IData data, Action<IData> onSuccessTask) {
        _popupShowChecker.OnReceivedData(data, onSuccessTask);
    }

    private void ActivateData(IData data) {
        _permissionController.CheckCameraMicAccess(() => {
            MenuConstructor.OnActivated?.Invoke(false);
            ARMsgRecordConstructor.OnActivated?.Invoke(false);
            StreamOverlayConstructor.onDeactivatedAsBroadcaster?.Invoke();

            if (data is ARMsgJSON.Data) {
                ARenaConstructor.onActivateForARMessaging?.Invoke(data as ARMsgJSON.Data);
                ARMsgARenaConstructor.OnActivatedARena?.Invoke(data as ARMsgJSON.Data);
            } else {
                ARenaConstructor.onActivateForPreRecorded?.Invoke(data as StreamJsonData.Data, false);
                PrerecordedVideoConstructor.OnActivated?.Invoke(data as StreamJsonData.Data);
            }

            PnlRecord.CurrentUser = data.GetUsername;
        });
    }

    private void ShowError(WebRequestError webRequestError) {
        _popupShowChecker.OnReceivedData(() => WarningConstructor.ActivateSingleButton("This user or video doesn't exist", "Please make sure that the link you received is correct.", "Ok"));
    }

}
