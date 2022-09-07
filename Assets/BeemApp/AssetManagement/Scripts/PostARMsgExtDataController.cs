using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Post ARMsgExtDataController
/// </summary>
public class PostARMsgExtDataController {
    private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;
    private WebRequestHandler _webRequestHandler;

    /// <summary>
    /// Constructor for PostARMsgExtDataController
    /// </summary>
    /// <param name="arMsgAPIScriptableObject"></param>
    /// <param name="webRequestHandler"></param>
    public PostARMsgExtDataController(ARMsgAPIScriptableObject arMsgAPIScriptableObject, WebRequestHandler webRequestHandler) {
        _arMsgAPIScriptableObject = arMsgAPIScriptableObject;
        _webRequestHandler = webRequestHandler;
    }

    /// <summary>
    /// Post AR Message Ext Data by Id
    /// </summary>
    /// <param name="id"></param>
    public void PostARMsgExtDataById(string id, ARMsgJSON.Data.ExtContentData extData, Action onSuccess = null, ErrorTypeDelegate onFailed = null) {
        _webRequestHandler.Post(PostARMsgExtById(id), extData, WebRequestBodyType.JSON,
            (code, body) => { onSuccess?.Invoke(); },
            (code, body) => { Debug.Log("PostARMsgExtDataById Error"); onFailed?.Invoke(code, body); });
    }


    private string PostARMsgExtById(string id) {
        return _webRequestHandler.ServerURLMediaAPI + _arMsgAPIScriptableObject.ARMessageExtData.Replace("{id}", id.ToString());
    }
}
