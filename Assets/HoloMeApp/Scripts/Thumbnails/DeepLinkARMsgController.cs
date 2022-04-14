using System;
using Beem.ARMsg;
using Beem.Firebase.DynamicLink;
using Firebase.DynamicLinks;
using UnityEngine;
using Zenject;

/// <summary>
/// Deep Link Controller for Ar-messages
/// </summary>
public class DeepLinkARMsgController : MonoBehaviour {

    [SerializeField]
    private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;

    private GetARMsgController _getARMsgController;

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler) {
        _getARMsgController = new GetARMsgController(_arMsgAPIScriptableObject, webRequestHandler);
    }

    private void OnOpen(string id) {
        _getARMsgController.GetARMsgById(id, DeeplinkARMsgConstructor.OnShow, DeeplinkARMsgConstructor.OnShowError);
    }

    private void Awake() {
        StreamCallBacks.onReceiveARMsgLink += OnOpen;
    }

    private void OnDestroy() {
        StreamCallBacks.onReceiveARMsgLink -= OnOpen;
    }

}
