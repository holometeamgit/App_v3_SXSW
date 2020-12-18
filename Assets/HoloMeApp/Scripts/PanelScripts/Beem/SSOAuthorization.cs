using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SSOAuthorization : MonoBehaviour
{
    [SerializeField] DeepLinkHandler deepLinkHandler;
    [SerializeField] AccountManager accountManager;
    [SerializeField] WebRequestHandler webRequestHandler;
    [SerializeField] AuthorizationAPIScriptableObject authorizationAPI;

    [SerializeField] UnityEvent OnAuthorized;

    public void AppleLogIn() {
        Application.OpenURL(GetAppleLogInRequest());
    }

    public void GoogleLogIn() {
        Application.OpenURL(GetGoogleLogInRequest());
    }

    public void FacebookLogIn() {

    }

    private void Awake() {
        deepLinkHandler.OnCompleteSSOLoginGetted += SSOLogIn;
    }

    private void SSOLogIn(ServerAccessToken serverAccessToken) {
        Debug.Log("SSOLogIn");
        if (serverAccessToken == null || accountManager.GetLoginType() != LogInType.None)
            return;


        accountManager.SaveAccessToken(JsonUtility.ToJson(serverAccessToken));
        accountManager.SaveLastAutoType(LogInType.SSO);

        OnAuthorized.Invoke();
    }

    private string GetAppleLogInRequest() {
        return webRequestHandler.ServerURLAuthAPI + authorizationAPI.AppleSSODeepLink;
    }

    private string GetGoogleLogInRequest() {
        return webRequestHandler.ServerURLAuthAPI + authorizationAPI.GoogleSSODeepLink;
    }

}
