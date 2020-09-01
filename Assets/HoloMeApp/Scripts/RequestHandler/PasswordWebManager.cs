using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasswordWebManager : MonoBehaviour
{
    [SerializeField] WebRequestHandler webRequestHandler;
    [SerializeField] AccountManager accountManager;
    [SerializeField] AuthorizationAPIScriptableObject authorizationAPI;

    public void ChangePassword(PasswordChangeJsonData passwordChangeJsonData, ResponseDelegate responseCallBack, ErrorTypeDelegate errorTypeCallBack) {
        webRequestHandler.PostRequest(GetRequestPasswordChangeURL(), passwordChangeJsonData, WebRequestHandler.BodyType.JSON,
            responseCallBack, errorTypeCallBack, accountManager.GetAccessToken().access);
    }

    private string GetRequestPasswordChangeURL() {
        return webRequestHandler.ServerURLAuthAPI + authorizationAPI.ChangePassword;
    }
}
