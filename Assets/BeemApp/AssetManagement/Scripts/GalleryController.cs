using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controller for get all messages
/// </summary>
public class GalleryController {
    private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;
    private WebRequestHandler _webRequestHandler;

    public GalleryController(ARMsgAPIScriptableObject arMsgAPIScriptableObject, WebRequestHandler webRequestHandler) {
        _arMsgAPIScriptableObject = arMsgAPIScriptableObject;
        _webRequestHandler = webRequestHandler;
    }

    /// <summary>
    /// Get All AR Messages
    /// </summary>
    /// <param name="page"></param>
    public void GetAllArMessages(int page = 1, Action<ARMsgJSON> onSuccess = null, Action onFailed = null) {
        _webRequestHandler.Get(GetRequestUserARMsgURL(page), (code, body) => { OnSuccess(code, body, onSuccess); }, (code, body) => { OnFailed(code, body, onFailed); });
    }

    private void OnSuccess(long code, string body, Action<ARMsgJSON> onSuccess) {
        ARMsgJSON arMsgJSON = JsonUtility.FromJson<ARMsgJSON>(body);
        if (arMsgJSON != null) {
            onSuccess?.Invoke(arMsgJSON);
        }
    }

    private void OnFailed(long code, string body, Action onFailed) {
        HelperFunctions.DevLogError(code + " " + body);
        onFailed?.Invoke();
    }

    private string GetRequestUserARMsgURL(int page) {
        return _webRequestHandler.ServerURLMediaAPI + _arMsgAPIScriptableObject.UserARMessages + $"?page={page}";
    }
}
