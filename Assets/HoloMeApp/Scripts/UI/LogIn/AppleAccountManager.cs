using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Beem.SSO;
using Zenject;

public class AppleAccountManager : MonoBehaviour {

    [Space]
    [SerializeField]
    AuthorizationAPIScriptableObject authorizationAPI;

    private WebRequestHandler _webRequestHandler;

    private IAppleAuthManager appleAuthManager;

    private string _accessToken;

    public UnityEvent OnAuthorized;

    private AccountManager _accountManager;

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler, AccountManager accountManager) {
        _webRequestHandler = webRequestHandler;
        _accountManager = accountManager;
    }

    public void SignInWithAppleButtonPressed() {

        HelperFunctions.DevLog("SignInWithAppleButtonPressed");

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

    public void AttemptQuickLogin() {
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

                HelperFunctions.DevLog("All info");

                HelperFunctions.DevLog("AuthorizationCode " + appleIdCredential.AuthorizationCode);
                HelperFunctions.DevLog("AuthorizationCode " + appleIdCredential.AuthorizationCode.Length);

                string code = System.Text.Encoding.ASCII.GetString(
                        appleIdCredential.AuthorizationCode,
                        0,
                        appleIdCredential.AuthorizationCode.Length);



                try {
                    HelperFunctions.DevLog(code);
                } catch (System.Exception ex) {
                    HelperFunctions.DevLogError(ex.Message);
                }

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



        HelperFunctions.DevLog("try Init Apple");

        if (!AppleAuthManager.IsCurrentPlatformSupported)
            return;

        HelperFunctions.DevLog("Apple Init");

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

        _accountManager.SaveAccessToken(_accessToken);
        HelperFunctions.DevLog("Save Acceess token: \n" + _accessToken);
        _accessToken = null;
    }

    private void SignInWithApple() {
        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);
        HelperFunctions.DevLog("LogIn WithApple");
        appleAuthManager.LoginWithAppleId(
            loginArgs,
            credential => {
                var appleIdCredential = credential as IAppleIDCredential;

                if (appleIdCredential == null)
                    return;

                HelperFunctions.DevLog("All info");

                HelperFunctions.DevLog("AuthorizationCode " + appleIdCredential.AuthorizationCode);
                HelperFunctions.DevLog("AuthorizationCode " + appleIdCredential.AuthorizationCode.Length);

                string code = "";
                try {
                    // Authorization code
                    code = System.Text.Encoding.UTF8.GetString(
                        appleIdCredential.AuthorizationCode,
                        0,
                        appleIdCredential.AuthorizationCode.Length);
                    HelperFunctions.DevLog(System.Text.Encoding.UTF8.GetString(appleIdCredential.IdentityToken));
                } catch (System.Exception ex) {
                    HelperFunctions.DevLogError(ex.Message);
                }

                GUIUtility.systemCopyBuffer = code;

                GetServerAccessToken(code, appleIdCredential.State, SuccessRequestAccessTokenCallBack, ErrorRequestAccessTokenCallBack);

                //AttemptQuickLogin();
            },
            error => {
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                HelperFunctions.DevLog("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
                HelperFunctions.DevLog("error.Domain: " + error.Domain);
                HelperFunctions.DevLog("error.LocalizedDescription: " + error.LocalizedDescription);
                HelperFunctions.DevLog("error.LocalizedFailureReason: " + error.LocalizedFailureReason);
                HelperFunctions.DevLog("error.LocalizedRecoveryOptions: " + error.LocalizedRecoveryOptions);
                HelperFunctions.DevLog("error.LocalizedRecoverySuggestion: " + error.LocalizedRecoverySuggestion);
            });
    }

    private void ErrorRequestAccessTokenCallBack(long code, string data) {
        HelperFunctions.DevLog(code + " : " + data);
    }

    private void SuccessRequestAccessTokenCallBack(long code, string data) {
        switch (code) {
            case 200:
                HelperFunctions.DevLog("Acceess token: \n" + data);
                _accessToken = data;
                SaveAccessTokens();
                _accountManager.SaveLogInType(LogInType.Apple);
                OnAuthorized.Invoke();
                break;
        }
    }


    #region request server access token
    private void GetServerAccessToken<T>(T appleAccessToken, string state, ResponseDelegate responseCallBack, ErrorTypeDelegate errorCallBack) {
        string url = GetGetRequestAccessTokenURL(appleAccessToken as string);
        HelperFunctions.DevLog(" Apple req " + url);
        HelperFunctions.DevLog(" appleAccessToken " + (appleAccessToken as string));
        Dictionary<string, T> formData = new Dictionary<string, T>();
        formData["code"] = appleAccessToken;
        //webRequestHandler.PostRequest(url, formData, WebRequestHandler.BodyType.XWWWFormUrlEncoded, responseCallBack, errorCallBack);
        _webRequestHandler.Get(GetGetRequestAccessTokenURL(appleAccessToken as string), responseCallBack, errorCallBack, needHeaderAccessToken: false);
    }

    private string GetRequestAccessTokenURL() {
        return _webRequestHandler.ServerURLAuthAPI + authorizationAPI.AppleCompliteLogIn;
    }

    private string GetGetRequestAccessTokenURL(string code) {
        return _webRequestHandler.ServerURLAuthAPI + authorizationAPI.AppleCompliteLogIn + "/?code=" + code;
    }
    #endregion

}
