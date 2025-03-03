using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Get MyBusinessProfile
/// </summary>
public class GetMyBusinessProfile {
    private AuthorizationAPIScriptableObject _authorizationAPIScriptableObject;
    private WebRequestHandler _webRequestHandler;

    /// <summary>
    /// Constructor for GetMyBusinessProfile
    /// </summary>
    /// <param name="arMsgAPIScriptableObject"></param>
    /// <param name="webRequestHandler"></param>
    public GetMyBusinessProfile(AuthorizationAPIScriptableObject authorizationAPIScriptableObject, WebRequestHandler webRequestHandler) {
        _authorizationAPIScriptableObject = authorizationAPIScriptableObject;
        _webRequestHandler = webRequestHandler;
    }

    /// <summary>
    /// Get My Business Profile
    /// </summary>
    /// <param name="id"></param>
    public void GetMyProfile(ResponseDelegate onSuccess, Action<WebRequestError> onFailed) {
        _webRequestHandler.Get(GetProfile(), onSuccess, (code, body) => OnFailed(code, body, onFailed));
    }

    private void OnSuccess(long code, string body, Action<BusinessProfileJsonData> onSuccess, Action<WebRequestError> onFailed) {
        BusinessProfileJsonData data = JsonUtility.FromJson<BusinessProfileJsonData>(body);
        if (data != null) {
            onSuccess?.Invoke(data);
        } else {
            onFailed?.Invoke(new WebRequestError());
        }
    }

    private void OnFailed(long code, string body, Action<WebRequestError> onFailed) {
        onFailed?.Invoke(new WebRequestError(code, body));
    }

    private string GetProfile() {
        return _webRequestHandler.ServerURLMediaAPI + _authorizationAPIScriptableObject.GetMyProfile;
    }
}
