using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasswordWebManager : MonoBehaviour
{

    [SerializeField] string passwordChangeAPI = "/password/change/";
    [SerializeField] WebRequestHandler webRequestHandler;
    [SerializeField] AccountManager accountManager;

    public void ChangePassword(PasswordChangeJsonData passwordChangeJsonData, ResponseDelegate responseCallBack, ErrorTypeDelegate errorTypeCallBack) {
        webRequestHandler.PostRequest(GetRequestPasswordChangeURL(), passwordChangeJsonData, WebRequestHandler.BodyType.JSON,
            responseCallBack, errorTypeCallBack, accountManager.GetAccessToken().access);
    }

    private string GetRequestPasswordChangeURL() {
        return webRequestHandler.serverURLAuthAPI + passwordChangeAPI;
    }


}
