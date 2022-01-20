using UnityEngine;
using Beem.ARMsg;
using System;

/// <summary>
/// ARMsgConstructor. TODO convert it to DI in future 
/// </summary>
public class ARMsgRecordConstructor : WindowConstructor {
    [SerializeField]
    private ARMsgAPIScriptableObject _arMsgAPI;
    [SerializeField]
    private WebRequestHandler _webRequestHandler;

    private ARMsgController _arMsgController;

    public static Action<bool> OnActivated = delegate { };


    private void Awake() {
        _arMsgController = new ARMsgController(_arMsgAPI, _webRequestHandler);

        CallBacks.OnUpdloadingUIOpened += _arMsgController.UploadARMsg;
        CallBacks.OnCancelAllARMsgActions += _arMsgController.OnCancelAll;
        CallBacks.OnARMsgProcessingCheck += _arMsgController.GetLastUploadedARMsgInfo;
        CallBacks.OnCheckContainLastUploadedARMsg += _arMsgController.CheckContainLastUploadedARMsg;
        CallBacks.OnDeleteLastARMsgActions += _arMsgController.DeleteLastARMsg;
        CallBacks.OnGetLastARMsgShareLink += _arMsgController.GetReadyShareLink;
        CallBacks.OnGetLastReadyARMsgData += _arMsgController.GetLastReadyARMsgData;
        CallBacks.OnCancelLastGetARMsgById += _arMsgController.OnCancelLastGetARMsgById;
        OnActivated += Activate;
    }

    private void OnDestroy() {
        _arMsgController?.OnCancelAll();

        CallBacks.OnUpdloadingUIOpened -= _arMsgController.UploadARMsg;
        CallBacks.OnCancelAllARMsgActions -= _arMsgController.OnCancelAll;
        CallBacks.OnARMsgProcessingCheck -= _arMsgController.GetLastUploadedARMsgInfo;
        CallBacks.OnCheckContainLastUploadedARMsg -= _arMsgController.CheckContainLastUploadedARMsg;
        CallBacks.OnDeleteLastARMsgActions -= _arMsgController.DeleteLastARMsg;
        CallBacks.OnGetLastARMsgShareLink -= _arMsgController.GetReadyShareLink;
        CallBacks.OnGetLastReadyARMsgData -= _arMsgController.GetLastReadyARMsgData;
        CallBacks.OnCancelLastGetARMsgById -= _arMsgController.OnCancelLastGetARMsgById;
        OnActivated -= Activate;
    }

    private void Activate(bool status) {
        _window?.SetActive(status);
    }
}
