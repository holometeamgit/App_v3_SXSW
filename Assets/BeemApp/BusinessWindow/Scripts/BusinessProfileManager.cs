using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Business Profile Manager
/// </summary>
public class BusinessProfileManager : MonoBehaviour {
    [SerializeField]
    private AuthorizationAPIScriptableObject _authorizationAPIScriptableObject;
    private WebRequestHandler _webRequestHandler;
    private GetMyBusinessProfile getMyBusinessProfile;

    private BusinessProfileData _data;

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler) {
        _webRequestHandler = webRequestHandler;
        getMyBusinessProfile = new GetMyBusinessProfile(_authorizationAPIScriptableObject, _webRequestHandler);
    }

    /// <summary>
    /// Get My Business Profile
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onFailed"></param>
    public void GetMyData(Action<BusinessProfileData> onSuccess, Action<WebRequestError> onFailed = null) {
        if (_data == null) {
            getMyBusinessProfile.GetMyProfile((data) => OnSuccess(data, onSuccess), onFailed);
        } else {
            onSuccess?.Invoke(_data);
        }
    }

    private void OnSuccess(BusinessProfileData data, Action<BusinessProfileData> onSuccess) {
        _data = data;
        onSuccess?.Invoke(_data);
    }
}
