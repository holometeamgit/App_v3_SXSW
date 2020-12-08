using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class AppleAccountManager : MonoBehaviour {
    [SerializeField]
    AccountManager accountManager;
    [SerializeField]
    WebRequestHandler webRequestHandler;

    [Space]
    [SerializeField]
    AuthorizationAPIScriptableObject authorizationAPI;

    private IAppleAuthManager appleAuthManager;

    private string _accessToken;

    public UnityEvent OnAuthorized;

    public void SignInWithAppleButtonPressed() {

        Debug.Log("SignInWithAppleButtonPressed");

        if (!AppleAuthManager.IsCurrentPlatformSupported)
            return;
        this.SignInWithApple();
    }

    public bool IsCurrentPlatformSupported() {
        return AppleAuthManager.IsCurrentPlatformSupported;
    }

    public  void AttemptQuickLogin() {
        if (!AppleAuthManager.IsCurrentPlatformSupported)
            return;

        var quickLoginArgs = new AppleAuthQuickLoginArgs();

        // Quick login should succeed if the credential was authorized before and not revoked
        this.appleAuthManager.QuickLogin(
            quickLoginArgs,
            credential => {
                // If it's an Apple credential, save the user ID, for later logins
                var appleIdCredential = credential as IAppleIDCredential;
                if (appleIdCredential != null)
                    GetServerAccessToken(credential.User, SuccessRequestAccessTokenCallBack, ErrorRequestAccessTokenCallBack);
            },
            error => {
                // If Quick Login fails, we should show the normal sign in with apple menu, to allow for a normal Sign In with apple
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                Debug.LogWarning("Quick Login Failed " + authorizationErrorCode.ToString() + " " + error.ToString());
            });
    }

    void Awake() {
        Init();
    }

    private void Init() {

        Debug.Log("try Init Apple");

        if (!AppleAuthManager.IsCurrentPlatformSupported)
            return;

        Debug.Log("Apple Init");

        var deserializer = new PayloadDeserializer();
        appleAuthManager = new AppleAuthManager(deserializer);
        //this.InitializeLoginMenu();
    }

    private void Update() {
        // Updates the AppleAuthManager instance to execute
        // pending callbacks inside Unity's execution loop
        if (appleAuthManager != null) {
            this.appleAuthManager.Update();
        }
    }

    private void SaveAccessTokens() {

        accountManager.SaveAccessToken(_accessToken);
        Debug.Log("Save Acceess token: \n" + _accessToken);
        _accessToken = null;
    }

    private void SignInWithApple() {
        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);
        Debug.Log("LogIn WithApple");
        appleAuthManager.LoginWithAppleId(
            loginArgs,
            credential => {
                var appleIdCredential = credential as IAppleIDCredential;
                Debug.Log("All info");

          /*      Debug.Log("AuthorizationCode " + appleIdCredential.AuthorizationCode.ToString());
                Debug.Log("AuthorizedScopes " + appleIdCredential.AuthorizedScopes.ToString());
                Debug.Log("Email " + appleIdCredential.Email);
                Debug.Log("FullName " + appleIdCredential.FullName.FamilyName);
                Debug.Log("IdentityToken " + appleIdCredential.IdentityToken.ToString());
                Debug.Log("RealUserStatus " + appleIdCredential.RealUserStatus);
                Debug.Log("State " + appleIdCredential.State);*/
                Debug.Log("User " + appleIdCredential.User);
                GetServerAccessToken(credential.User, SuccessRequestAccessTokenCallBack, ErrorRequestAccessTokenCallBack);
            },
            error => {
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                Debug.LogWarning("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
            });
    }

    private void ErrorRequestAccessTokenCallBack(long code, string data) {
        Debug.Log(code + " : " + data);
    }

    private void SuccessRequestAccessTokenCallBack(long code, string data) {
        switch (code) {
            case 200:
                Debug.Log("Acceess token: \n" + data);
                _accessToken = data;
                SaveAccessTokens();
                accountManager.SaveLastAutoType(LogInType.Apple);
                OnAuthorized.Invoke();
                break;
        }
    }


    #region request server access token
    private void GetServerAccessToken(string appleAccessToken, ResponseDelegate responseCallBack, ErrorTypeDelegate errorCallBack) {
        string url = GetRequestAccessTokenURL(appleAccessToken);
        Debug.Log(" Apple post req " + url);
        Dictionary<string, string> formData = new Dictionary<string, string>();
        formData["code"] = appleAccessToken;
        webRequestHandler.PostRequest(url, formData, WebRequestHandler.BodyType.XWWWFormUrlEncoded, responseCallBack, errorCallBack);
    }

    private string GetRequestAccessTokenURL(string appleAccessToken) {
        return webRequestHandler.ServerURLAuthAPI + authorizationAPI.AppleCompliteLogIn;
    }
    #endregion

}
