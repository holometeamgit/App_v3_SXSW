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
    public void GetARMsgById(string id, Action<ARMsgJSON.Data> onSuccess = null, Action<WebRequestError> onFailed = null) {
        _webRequestHandler.Get(GetARMsgById(id), (code, body) => { OnSuccess(code, body, onSuccess); }, (code, body) => { OnFailed(code, body, onFailed); });
    }

    private void OnSuccess(long code, string body, Action<ARMsgJSON.Data> onSuccess) {
        ARMsgJSON.Data data = JsonUtility.FromJson<ARMsgJSON.Data>(body);
        if (data != null) {
            onSuccess?.Invoke(data);
        }
    }

    private void OnFailed(long code, string body, Action<WebRequestError> onFailed) {
        HelperFunctions.DevLogError("Failed" + code + " " + body);
        onFailed?.Invoke(new WebRequestError(code, body));
    }

    private string GetARMsgById(string id) {
        return _webRequestHandler.ServerURLMediaAPI + _arMsgAPIScriptableObject.ARMessageById.Replace("{id}", id.ToString());
    }
}
