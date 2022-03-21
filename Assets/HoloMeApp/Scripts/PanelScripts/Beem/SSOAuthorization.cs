using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Beem.SSO;
using Zenject;

public class SSOAuthorization : MonoBehaviour {
    [SerializeField] DeepLinkHandler deepLinkHandler;
    [SerializeField] AccountManager accountManager;
    [SerializeField] AuthorizationAPIScriptableObject authorizationAPI;

    [SerializeField] UnityEvent OnAuthorized;

    private WebRequestHandler _webRequestHandler;

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler) {
        _webRequestHandler = webRequestHandler;
    }

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
        OnAuthorized.AddListener(() => AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyRegistrationComplete));
    }

    private void SSOLogIn(ServerAccessToken serverAccessToken) {
        Debug.Log("SSOLogIn");
        if (serverAccessToken == null || accountManager.GetLogInType() != LogInType.None)
            return;


        accountManager.SaveAccessToken(JsonUtility.ToJson(serverAccessToken));
        accountManager.SaveLogInType(LogInType.SSO);

        OnAuthorized.Invoke();
    }

    private string GetAppleLogInRequest() {
        return _webRequestHandler.ServerURLAuthAPI + authorizationAPI.AppleSSODeepLink;
    }

    private string GetGoogleLogInRequest() {
        return _webRequestHandler.ServerURLAuthAPI + authorizationAPI.GoogleSSODeepLink;
    }

}
