using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Delete ARMsgController
/// </summary>
public class DeleteARMsgController {
    private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;
    private WebRequestHandler _webRequestHandler;

    public DeleteARMsgController(ARMsgAPIScriptableObject arMsgAPIScriptableObject, WebRequestHandler webRequestHandler) {
        _arMsgAPIScriptableObject = arMsgAPIScriptableObject;
        _webRequestHandler = webRequestHandler;
    }

    /// <summary>
    /// Delete AR Message
    /// </summary>
    /// <param name="id"></param>
    public void DeleteARMessages(string id, Action onSuccess = null, Action onFailed = null) {
        _webRequestHandler.Delete(GetRequestDeleteARMsgByIdURL(id), (code, body) => { OnSuccess(code, body, onFailed); }, (code, body) => { OnFailed(code, body, onFailed); });
    }

    private void OnSuccess(long code, string body, Action onSuccess) {
        onSuccess?.Invoke();
    }

    private void OnFailed(long code, string body, Action onFailed) {
        HelperFunctions.DevLogError(code + " " + body);
        onFailed?.Invoke();
    }

    private string GetRequestDeleteARMsgByIdURL(string id) {
        return _webRequestHandler.ServerURLMediaAPI + _arMsgAPIScriptableObject.DeleteARMessageById.Replace("{id}", id.ToString());
    }
}
