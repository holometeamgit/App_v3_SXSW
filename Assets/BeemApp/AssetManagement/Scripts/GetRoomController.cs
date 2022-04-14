using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Get RoomController
/// </summary>
public class GetRoomController {
    private VideoUploader _videoUploaderScriptableObject;
    private WebRequestHandler _webRequestHandler;

    /// <summary>
    /// Constructor for GetRoomController
    /// </summary>
    /// <param name="arMsgAPIScriptableObject"></param>
    /// <param name="webRequestHandler"></param>
    public GetRoomController(VideoUploader videoUploaderScriptableObject, WebRequestHandler webRequestHandler) {
        _videoUploaderScriptableObject = videoUploaderScriptableObject;
        _webRequestHandler = webRequestHandler;
    }

    /// <summary>
    /// Get Room By username
    /// </summary>
    /// <param name="id"></param>
    public void GetRoomByUsername(string username, Action<RoomJsonData> onSuccess = null, Action<WebRequestError> onFailed = null) {
        _webRequestHandler.Get(GetRoomByUsername(username), (code, body) => { OnSuccess(code, body, onSuccess); }, (code, body) => { OnFailed(code, body, onFailed); });
    }

    private void OnSuccess(long code, string body, Action<RoomJsonData> onSuccess) {
        RoomJsonData data = JsonUtility.FromJson<RoomJsonData>(body);
        if (data != null) {
            onSuccess?.Invoke(data);
        }
    }

    private void OnFailed(long code, string body, Action<WebRequestError> onFailed) {
        HelperFunctions.DevLogError("Failed" + code + " " + body);
        onFailed?.Invoke(new WebRequestError(code, body));
    }

    private string GetRoomByUsername(string username) {
        return _webRequestHandler.ServerURLMediaAPI + _videoUploaderScriptableObject.GetRoomByUserName.Replace("{username}", username.ToString());
    }
}
