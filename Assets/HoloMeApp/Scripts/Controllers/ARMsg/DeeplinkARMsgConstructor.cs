using Beem.Permissions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WindowManager.Extenject;

/// <summary>
/// Constructor for opening deep Link for ARMessage
/// </summary>
public class DeepLinkARMsgConstructor : MonoBehaviour {

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
            HomeConstructor.OnHide?.Invoke();
            BottomMenuConstructor.OnHide?.Invoke();
            SettingsConstructor.OnHide?.Invoke();
            ARMsgRecordConstructor.OnHide?.Invoke();
            ARenaConstructor.OnShowARMessaging?.Invoke(data);
            ARMsgARenaConstructor.OnShow?.Invoke(data);
            PnlRecord.CurrentUser = data.user;
        });
    }

    private void ShowError(WebRequestError webRequestError) {
        GeneralPopUpData.ButtonData closeButton = new GeneralPopUpData.ButtonData("Ok", null);
        GeneralPopUpData data = new GeneralPopUpData("This user or video doesn't exist", "Please make sure that the link you received is correct.", closeButton);
        _popupShowChecker.OnReceivedData(() => WarningConstructor.OnShow?.Invoke(data));
    }
}

