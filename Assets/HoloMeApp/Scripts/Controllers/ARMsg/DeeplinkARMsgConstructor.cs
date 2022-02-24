using Beem.Permissions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Constructor for opening deep Link for ARMessage
/// </summary>
public class DeeplinkARMsgConstructor : MonoBehaviour {

    [SerializeField]
    private DeepLinkChecker _popupShowChecker;

    private PermissionController _permissionController = new PermissionController();

    public static Action<ARMsgJSON.Data> OnShow = delegate { };
    public static Action<WebRequestError> OnShowError = delegate { };

    private void OnEnable() {
        OnShow += Show;
        OnShowError += ShowError;
    }

    private void OnDisable() {
        OnShow -= Show;
        OnShowError -= ShowError;
    }

    private void Show(ARMsgJSON.Data data) {
        OnReceivedARMessageData(data, ActivateData);
    }

    private void OnReceivedARMessageData(ARMsgJSON.Data data, Action<ARMsgJSON.Data> onSuccessTask) {
        _popupShowChecker.OnReceivedData(data, onSuccessTask);
    }

    private void ActivateData(ARMsgJSON.Data data) {
        _permissionController.CheckCameraMicAccess(() => {
            MenuConstructor.OnActivated?.Invoke(false);
            SettingsConstructor.OnActivated?.Invoke(false);
            ARMsgRecordConstructor.OnActivated?.Invoke(false);
            ARenaConstructor.onActivateForARMessaging?.Invoke(data);
            ARMsgARenaConstructor.OnActivatedARena?.Invoke(data);
            PnlRecord.CurrentUser = data.user;
        });
    }

    private void ShowError(WebRequestError webRequestError) {
        _popupShowChecker.OnReceivedData(() => WarningConstructor.ActivateSingleButton("This user or video doesn't exist", "Please make sure that the link you received is correct.", "Ok"));
    }

}
