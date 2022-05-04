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
    public void GetMyProfile(Action<BusinessProfileData> onSuccess = null, Action<WebRequestError> onFailed = null) {
        _webRequestHandler.Get(GetProfile(), (code, body) => OnSuccess(code, body, onSuccess, onFailed), (code, body) => OnFailed(code, body, onFailed));
    }

    private void OnSuccess(long code, string body, Action<BusinessProfileData> onSuccess, Action<WebRequestError> onFailed) {

        BusinessProfileData data = JsonUtility.FromJson<BusinessProfileData>(body);
        if (data != null) {
            onSuccess?.Invoke(data);
        } else {
            onFailed?.Invoke(new WebRequestError());
        }
    }

    private void OnFailed(long code, string body, Action<WebRequestError> onFailed) {
        HelperFunctions.DevLogError("Failed" + code + " " + body);
        onFailed?.Invoke(new WebRequestError(code, body));
    }

    private string GetProfile() {
        return _webRequestHandler.ServerURLMediaAPI + _authorizationAPIScriptableObject.GetMyProfile;
    }
}
