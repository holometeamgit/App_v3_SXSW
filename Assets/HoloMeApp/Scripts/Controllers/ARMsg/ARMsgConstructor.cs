using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Beem.ARMsg;

public class ARMsgConstructor : MonoBehaviour {
    [SerializeField]
    private ARMsgAPIScriptableObject _arMsgAPI;
    [SerializeField]
    private WebRequestHandler _webRequestHandler;
    [SerializeField]
    private PnlARMessages pnlARMessages;

    private ARMsgController _arMsgController;


    private void Activate(ARMsgJSON.Data data) {
        pnlARMessages.Init(data);
    }

    private void Deactivate() {
        pnlARMessages.Deactivate();
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
        StreamCallBacks.onARMsgLinkReceived -= _arMsgController.GetARMsgById;
        CallBacks.OnActivated -= Activate;
        CallBacks.OnDeactivated -= Deactivate;
        CallBacks.OnGetLastReadyARMsgData -= _arMsgController.GetLastReadyARMsgData;

        _arMsgController?.OnCancelAll();
    }
}
