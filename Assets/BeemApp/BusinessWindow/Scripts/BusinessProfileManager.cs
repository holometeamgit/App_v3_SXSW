using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Beem.SSO;

/// <summary>
/// Business Profile Manager
/// </summary>
public class BusinessProfileManager : MonoBehaviour {
    private AuthorizationAPIScriptableObject _authorizationAPIScriptableObject;
    private WebRequestHandler _webRequestHandler;
    private GetMyBusinessProfile getMyBusinessProfile;

    private BusinessProfileJsonData _data;

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler, AuthorizationAPIScriptableObject authorizationAPIScriptableObject) {
        _webRequestHandler = webRequestHandler;
        _authorizationAPIScriptableObject = authorizationAPIScriptableObject;
        getMyBusinessProfile = new GetMyBusinessProfile(_authorizationAPIScriptableObject, _webRequestHandler);
    }

    /// <summary>
    /// Get My Business Profile
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onFailed"></param>
    public void GetMyData(Action<BusinessProfileJsonData> onSuccess, Action<WebRequestError> onFailed = null, bool forceUpdate = false) {
        if (_data == null || forceUpdate) {
            getMyBusinessProfile.GetMyProfile((data) => OnSuccess(data, onSuccess), onFailed);
        } else {
            onSuccess?.Invoke(_data);
        }
    }

    public string GetID() {
        return _data == null ? null : _data.id;
    }

    public bool IsBusinessProfile() {
        return _data != null;
    }

    public string GetCTALable() {
        return _data == null ? null : _data.cta_label;
    }

    private void OnSuccess(BusinessProfileJsonData data, Action<BusinessProfileJsonData> onSuccess) {
        _data = data;
        onSuccess?.Invoke(_data);
        CallBacks.onBusinessDataUpdated?.Invoke();
    }
}
