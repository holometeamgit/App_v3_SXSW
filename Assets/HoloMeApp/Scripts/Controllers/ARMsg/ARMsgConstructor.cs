using UnityEngine;
using Beem.ARMsg;

public class ARMsgConstructor : MonoBehaviour {
    [SerializeField]
    private ARMsgAPIScriptableObject _arMsgAPI;
    [SerializeField]
    private WebRequestHandler _webRequestHandler;

    private ARMsgController _arMsgController;


    void Awake() {
        _arMsgController = new ARMsgController(_arMsgAPI, _webRequestHandler);

        CallBacks.OnUpdloadingUIOpened += _arMsgController.UploadARMsg;
        CallBacks.OnCancelAllARMsgActions += _arMsgController.OnCancelAll;
        CallBacks.OnARMsgProcessingCheck += _arMsgController.GetLastUploadedARMsgInfo;
        CallBacks.OnCheckContainLastUploadedARMsg += _arMsgController.CheckContainLastUploadedARMsg;
        CallBacks.OnDeleteLastARMsgActions += _arMsgController.DeleteLastARMsg;
        CallBacks.OnGetLastARMsgShareLink += _arMsgController.GetReadyShareLink;
        CallBacks.OnGetLastReadyARMsgData += _arMsgController.GetLastReadyARMsgData;
    }

    private void OnDestroy() {
        CallBacks.OnUpdloadingUIOpened -= _arMsgController.UploadARMsg;
        CallBacks.OnCancelAllARMsgActions -= _arMsgController.OnCancelAll;
        CallBacks.OnARMsgProcessingCheck -= _arMsgController.GetLastUploadedARMsgInfo;
        CallBacks.OnCheckContainLastUploadedARMsg -= _arMsgController.CheckContainLastUploadedARMsg;
        CallBacks.OnDeleteLastARMsgActions -= _arMsgController.DeleteLastARMsg;
        CallBacks.OnGetLastARMsgShareLink -= _arMsgController.GetReadyShareLink;
        CallBacks.OnGetLastReadyARMsgData -= _arMsgController.GetLastReadyARMsgData;

        _arMsgController?.OnCancelAll();
    }
}
