using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Get ARMsgController
/// </summary>
public class GetARMsgController {
    private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;
    private WebRequestHandler _webRequestHandler;

    /// <summary>
    /// Constructor for GetARMsgController
    /// </summary>
    /// <param name="arMsgAPIScriptableObject"></param>
    /// <param name="webRequestHandler"></param>
    public GetARMsgController(ARMsgAPIScriptableObject arMsgAPIScriptableObject, WebRequestHandler webRequestHandler) {
        _arMsgAPIScriptableObject = arMsgAPIScriptableObject;
        _webRequestHandler = webRequestHandler;
    }

    /// <summary>
    /// Get AR Message
    /// </summary>
    /// <param name="id"></param>
    public void GetARMessage(string id, Action<ARMsgJSON.Data> onSuccess = null, Action onFailed = null) {
        _webRequestHandler.Get(GetARMessageById(id), (code, body) => { OnSuccess(code, body, onSuccess); }, (code, body) => { OnFailed(code, body, onFailed); });
    }

    private void OnSuccess(long code, string body, Action<ARMsgJSON.Data> onSuccess) {
        ARMsgJSON.Data arMsgJSON = JsonUtility.FromJson<ARMsgJSON.Data>(body);
        if (arMsgJSON != null) {
            onSuccess?.Invoke(arMsgJSON);
        }
    }

    private void OnFailed(long code, string body, Action onFailed) {
        HelperFunctions.DevLogError("Failed" + code + " " + body);
        onFailed?.Invoke();
    }

    private string GetARMessageById(string id) {
        return _webRequestHandler.ServerURLMediaAPI + _arMsgAPIScriptableObject.ARMessageById.Replace("{id}", id.ToString());
    }
}
