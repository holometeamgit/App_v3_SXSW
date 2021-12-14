using UnityEngine;
using Beem.ARMsg;

/// <summary>
/// ARMsgConstructor. TODO convert it to DI in future 
/// </summary>
public class ARMsgConstructor : MonoBehaviour {
    [SerializeField]
    private ARMsgAPIScriptableObject _arMsgAPI;
    [SerializeField]
    private WebRequestHandler _webRequestHandler;

    private ARMsgController _arMsgController;


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
    }
}
