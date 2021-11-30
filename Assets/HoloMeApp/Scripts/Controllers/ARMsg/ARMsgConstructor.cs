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
    private PermissionController _permissionController;
    [SerializeField]
    private PnlARMessages _pnlARMessages;
    [SerializeField]
    private PnlViewingExperience _pnlViewingExperience;

    private ARMsgController _arMsgController;


    private void Activate(ARMsgJSON.Data data) {
        if (!_permissionController.CheckCameraAccess()) {
            return;
        }

        _pnlViewingExperience.ActivateForARMessaging(data);
        _pnlARMessages.Init(data);
    }

    private void Deactivate() {
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
        StreamCallBacks.onARMsgLinkReceived += _arMsgController.GetARMsgById;
        CallBacks.OnARMsgByIdReceived += Activate;
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
        StreamCallBacks.onARMsgLinkReceived -= _arMsgController.GetARMsgById;
        CallBacks.OnARMsgByIdReceived -= Activate;
        CallBacks.OnDeactivated -= Deactivate;
        CallBacks.OnGetLastReadyARMsgData -= _arMsgController.GetLastReadyARMsgData;

        _arMsgController?.OnCancelAll();
    }
}
