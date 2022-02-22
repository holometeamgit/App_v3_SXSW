using System;
using Beem.ARMsg;
using Beem.Firebase.DynamicLink;
using Firebase.DynamicLinks;
using UnityEngine;

/// <summary>
/// Deep Link Controller for Ar-messages
/// </summary>
public class DeepLinkARMsgController : MonoBehaviour {

    [SerializeField]
    private WebRequestHandler _webRequestHandler;

    [SerializeField]
    private ServerURLAPIScriptableObject _serverURLAPIScriptableObject;

    [SerializeField]
    private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;

    private void GetARMessageById(string id, Action<long, string> onSuccess, Action<WebRequestError> onFailed = null) {
        _webRequestHandler.Get(GetARMessageIdUrl(id),
            (code, body) => { onSuccess?.Invoke(code, body); },
            (code, body) => { onFailed?.Invoke(new WebRequestError(code, body)); },
            false);
    }

    private void ARMessageReceived(string body, Action<ARMsgJSON.Data> onSuccess, Action<WebRequestError> onFailed = null) {
        try {
            ARMsgJSON.Data arMsgJsonData = JsonUtility.FromJson<ARMsgJSON.Data>(body);

            HelperFunctions.DevLog("AR Message Recieved = " + body);

            onSuccess?.Invoke(arMsgJsonData);
        } catch (Exception e) {
            HelperFunctions.DevLogError(e.Message);
            onFailed?.Invoke(new WebRequestError());
        }
    }

    private void OnOpen(string id) {
        GetARMessageById(id,
            (code, body) => Open(body),
            DeeplinkARMsgConstructor.OnShowError);
    }

    private void Open(string body) {
        ARMessageReceived(body,
            (data) => {
                DeeplinkARMsgConstructor.OnShow?.Invoke(data);
            },
            DeeplinkARMsgConstructor.OnShowError);
    }

    private void Awake() {
        StreamCallBacks.onReceiveARMsgLink += OnOpen;
    }

    private void OnDestroy() {
        StreamCallBacks.onReceiveARMsgLink -= OnOpen;
    }

    private string GetARMessageIdUrl(string id) {
        return _serverURLAPIScriptableObject.ServerURLMediaAPI + _arMsgAPIScriptableObject.ARMessageById.Replace("{id}", id.ToString());
    }
}
