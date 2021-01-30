using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Beem.SSO;

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
            SignInWithApple();

        //AttemptQuickLogin();
        //second plugin
        //signInWithApple.Login(OnLogin);
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
                if (appleIdCredential == null)
                    return;

                Debug.Log("All info");

                Debug.Log("AuthorizationCode " + appleIdCredential.AuthorizationCode);
                Debug.Log("AuthorizationCode " + appleIdCredential.AuthorizationCode.Length);

                string code = System.Text.Encoding.ASCII.GetString(
                        appleIdCredential.AuthorizationCode,
                        0,
                        appleIdCredential.AuthorizationCode.Length);

                

                try {
                    Debug.Log(code);
                } catch (System.Exception ex) { }

                GUIUtility.systemCopyBuffer = code;

                GetServerAccessToken(code, appleIdCredential.State, SuccessRequestAccessTokenCallBack, ErrorRequestAccessTokenCallBack);
            },
            error => {
                // If Quick Login fails, we should show the normal sign in with apple menu, to allow for a normal Sign In with apple
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                Debug.LogWarning("Quick Login Failed " + authorizationErrorCode.ToString() + " " + error.ToString());
            });
    }
    
    void Start() {
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
                
                if (appleIdCredential == null)
                    return;

                Debug.Log("All info");

                Debug.Log("AuthorizationCode " + appleIdCredential.AuthorizationCode);
                Debug.Log("AuthorizationCode " + appleIdCredential.AuthorizationCode.Length);

               /* try {
                    Debug.Log("Scopes");
                    if (appleIdCredential.AuthorizedScopes == null)
                        foreach (var scope in appleIdCredential.AuthorizedScopes)
                            Debug.Log(scope);
                } catch (System.Exception) { }

                try { 
                Debug.Log("after  Scopes");
                Debug.Log("appleIdCredential.State " + appleIdCredential.State);
                } catch (System.Exception) { }
               */
                string code = "";
                try {
                    // Authorization code
                    code = System.Text.Encoding.UTF8.GetString(
                        appleIdCredential.AuthorizationCode,
                        0,
                        appleIdCredential.AuthorizationCode.Length);
                    Debug.Log(System.Text.Encoding.UTF8.GetString(appleIdCredential.IdentityToken));
                } catch (System.Exception ex) { }

                GUIUtility.systemCopyBuffer = code;

                GetServerAccessToken(code, appleIdCredential.State, SuccessRequestAccessTokenCallBack, ErrorRequestAccessTokenCallBack);

                //AttemptQuickLogin();
            },
            error => {
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                Debug.Log("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
                Debug.Log("error.Domain: " + error.Domain);
                Debug.Log("error.LocalizedDescription: " + error.LocalizedDescription);
                Debug.Log("error.LocalizedFailureReason: " + error.LocalizedFailureReason);
                Debug.Log("error.LocalizedRecoveryOptions: " + error.LocalizedRecoveryOptions);
                Debug.Log("error.LocalizedRecoverySuggestion: " + error.LocalizedRecoverySuggestion);
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
                accountManager.SaveLogInType(LogInType.Apple);
                OnAuthorized.Invoke();
                break;
        }
    }


    #region request server access token
    private void GetServerAccessToken<T>(T appleAccessToken, string state, ResponseDelegate responseCallBack, ErrorTypeDelegate errorCallBack) {
        string url = GetGetRequestAccessTokenURL(appleAccessToken as string);
        Debug.Log(" Apple req " + url);
        Debug.Log(" appleAccessToken " + (appleAccessToken as string));
        Dictionary<string, T> formData = new Dictionary<string, T>();
        formData["code"] = appleAccessToken;
        //webRequestHandler.PostRequest(url, formData, WebRequestHandler.BodyType.XWWWFormUrlEncoded, responseCallBack, errorCallBack);
        webRequestHandler.GetRequest(GetGetRequestAccessTokenURL(appleAccessToken as string), responseCallBack, errorCallBack);
    }

    private string GetRequestAccessTokenURL() {
        return webRequestHandler.ServerURLAuthAPI + authorizationAPI.AppleCompliteLogIn;
    }

    private string GetGetRequestAccessTokenURL(string code) {
        return webRequestHandler.ServerURLAuthAPI + authorizationAPI.AppleCompliteLogIn + "/?code=" + code;
    }
    #endregion

}
