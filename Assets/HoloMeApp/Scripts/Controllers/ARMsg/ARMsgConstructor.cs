using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Beem.ARMsg;
using Beem.Permissions;

public class ARMsgConstructor : MonoBehaviour {
    [SerializeField]
    private ARMsgAPIScriptableObject _arMsgAPI;
    [SerializeField]
    private WebRequestHandler _webRequestHandler;
    [SerializeField]
    private UIThumbnailsController _uiThumbnailsController;
    [SerializeField]
    private PermissionController _permissionController;
    [SerializeField]
    private PnlARMessages _pnlARMessages;
    [SerializeField]
    private PnlViewingExperience _pnlViewingExperience;

    private ARMsgController _arMsgController;

    private ARMsgJSON.Data currentData;


    private void Activate(ARMsgJSON.Data data) {

        if (!_permissionController.CheckCameraAccess()) {
            return;
        }

        currentData = data;

        _pnlViewingExperience.ActivateForARMessaging(data);

        _pnlARMessages.Init(data);

        _uiThumbnailsController.OnPlayFromUser?.Invoke(data.user);
    }

    private void Deactivate() {
        CallBacks.OnActivateGenericErrorDoubleButton?.Invoke("Before you go...",
           "If you exit you could lose your AR message if you don’t share the link.",
           "Copy link and exit", "Return",
           () => {
               GUIUtility.systemCopyBuffer = currentData.share_link;
               Close();
           },
           () => {
               Close();
           },
           false);

    }

    private void Close() {
        CallBacks.OnCancelAllARMsgActions?.Invoke();
        _pnlViewingExperience.StopExperience();
        _pnlARMessages.Deactivate();
    }


    void Awake() {
        _arMsgController = new ARMsgController(_arMsgAPI, _webRequestHandler);

        CallBacks.OnUpdloadingUIOpened += _arMsgController.UploadARMsg;
        CallBacks.OnCancelAllARMsgActions += _arMsgController.OnCancelAll;
        CallBacks.OnARMsgProcessingCheck += _arMsgController.GetLastUploadedARMsgInfo;
        CallBacks.OnCheckContainLastUploadedARMsg += _arMsgController.CheckContainLastUploadedARMsg;
        CallBacks.OnDeleteLastARMsgActions += _arMsgController.DeleteLastARMsg;
        CallBacks.OnGetLastARMsgShareLink += _arMsgController.GetReadyShareLink;
        CallBacks.OnActivated += Activate;
        CallBacks.OnDeactivated += Deactivate;
        CallBacks.OnGetLastReadyARMsgData += _arMsgController.GetLastReadyARMsgData;
    }

    private void OnDestroy() {
        CallBacks.OnUpdloadingUIOpened -= _arMsgController.UploadARMsg;
        CallBacks.OnCancelAllARMsgActions -= _arMsgController.OnCancelAll;
        CallBacks.OnARMsgProcessingCheck -= _arMsgController.GetLastUploadedARMsgInfo;
        CallBacks.OnCheckContainLastUploadedARMsg -= _arMsgController.CheckContainLastUploadedARMsg;
        CallBacks.OnDeleteLastARMsgActions -= _arMsgController.DeleteLastARMsg;
        CallBacks.OnGetLastARMsgShareLink -= _arMsgController.GetReadyShareLink;
        CallBacks.OnActivated -= Activate;
        CallBacks.OnDeactivated -= Deactivate;
        CallBacks.OnGetLastReadyARMsgData -= _arMsgController.GetLastReadyARMsgData;

        _arMsgController?.OnCancelAll();
    }
}
