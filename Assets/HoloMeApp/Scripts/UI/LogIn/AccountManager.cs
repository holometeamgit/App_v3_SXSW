using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Beem.SSO;
using System.Threading.Tasks;
using Zenject;

public class AccountManager : MonoBehaviour {

    private AuthorizationAPIScriptableObject _authorizationAPI;

    [Tooltip("Use this to test multiple editor logins on the same PC")]
    [SerializeField]
    bool disablePersistance;

    private ActionWrapper _onCancelLogIn;

    private WebRequestHandler _webRequestHandler;
    private AuthController _authController;

    private bool canLogIn = true;
    private const int QUICK_LOGIN_DELAY_TIME = 1000;

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler, AuthController authController, AuthorizationAPIScriptableObject authorizationAPI) {
        _webRequestHandler = webRequestHandler;
        _authController = authController;
        _authorizationAPI = authorizationAPI;
    }


    #region public authorization

    public void CancelLogIn() {
        _onCancelLogIn.InvokeAction();
    }

    public void LogOut() {
        RemoveAccessToken();
        CallBacks.onSignOut?.Invoke();
        SaveLogInType(LogInType.None);
    }

    /// <summary>
    /// check that user is Authorized
    /// </summary>
    public bool IsAuthorized() {
        return GetLogInType() != LogInType.None;
    }


    #endregion

    #region Auth Type

    public void SaveLogInType(LogInType type) {
        PlayerPrefs.SetInt(PlayerPrefsKeys.LastTypeLoginPPKey, (int)type);
        PlayerPrefs.Save();
    }

    public LogInType GetLogInType() {
        if (!PlayerPrefs.HasKey(PlayerPrefsKeys.LastTypeLoginPPKey)) {
            return LogInType.None;
        }
        return (LogInType)PlayerPrefs.GetInt(PlayerPrefsKeys.LastTypeLoginPPKey);
    }

    #endregion

    #region server access token

    ServerAccessToken temporaryEditorTestingAccessToken;

    public void SaveAccessToken(string serverAccessToken) {
        try {
            ServerAccessToken accessToken = JsonUtility.FromJson<ServerAccessToken>(serverAccessToken);
            HelperFunctions.DevLog("Save serverAccessToken " + serverAccessToken);
            FileAccountManager.SaveFile(nameof(FileAccountManager.ServerAccessToken), accessToken, FileAccountManager.ServerAccessToken);

            if (Application.isEditor && disablePersistance) {
                temporaryEditorTestingAccessToken = accessToken;
            }
        } catch (System.Exception e) {
            HelperFunctions.DevLogError(e.Message);
        }
    }

    public ServerAccessToken GetAccessToken() {
        if (Application.isEditor && disablePersistance) {
            return temporaryEditorTestingAccessToken;
        }

        return FileAccountManager.ReadFile<ServerAccessToken>(nameof(FileAccountManager.ServerAccessToken),
            FileAccountManager.ServerAccessToken);
    }

    private void Awake() {
        canLogIn = true;
        CallBacks.onQuickLogInRequest += QuickLogInWithDelay;
        CallBacks.onLogOutRequest += LogOut;
    }

    private void QuickLogIn() {
        _onCancelLogIn?.InvokeAction();

        _onCancelLogIn = new ActionWrapper();

        if (!canLogIn) {
            LogOut(); // hotfix v4.9
            return;
        }
        if (GetLogInType() == LogInType.Facebook) {
            LogOut();
        }

        ServerAccessToken accessToken = GetAccessToken();

        if (accessToken == null && !_authController.HasUser()) {
            ErrorRequestAccessTokenCallBack(0, "");// "Server Access Token file doesn't exist");
            //errorTypeCallBack?.Invoke();
            LogOut();
            return;
        } else if (accessToken == null && _authController.HasUser() && GetLogInType() != LogInType.None) {
            HelperFunctions.DevLog("Has firebase user. QuickLogIn Firebase");
            _authController.DoAfterReloadUser(() => CallBacks.onFirebaseSignInSuccess(GetLogInType())); //TODO need test 
            return;
        } else if (accessToken == null && _authController.HasUser() && GetLogInType() == LogInType.None) {
            HelperFunctions.DevLog("Has firebase user but doesn't have LogInType");
            LogOut();
            ErrorRequestAccessTokenCallBack(0, "");// "Server Access Token file doesn't exist");
            return;
        }

        canLogIn = false;
        _webRequestHandler.Post(GetRequestRefreshTokenURL(),
            accessToken, WebRequestBodyType.JSON,
            (code, body) => { UpdateAccessToken(body); SuccessRequestAccessTokenCallBack(code, body); },
            ErrorRequestAccessTokenCallBack, onCancel: _onCancelLogIn, needHeaderAccessToken: false);
    }

    private void UpdateAccessToken(string serverAccessToken) {
        try {
            ServerAccessToken currentAccessToken = GetAccessToken();
            if (currentAccessToken == null)
                return;
            ServerAccessToken accessToken = JsonUtility.FromJson<ServerAccessToken>(serverAccessToken);
            currentAccessToken.access = accessToken.access;

            HelperFunctions.DevLog("Updated serverAccessToken " + serverAccessToken);
            FileAccountManager.SaveFile(nameof(FileAccountManager.ServerAccessToken), currentAccessToken, FileAccountManager.ServerAccessToken);
        } catch (System.Exception e) {
            HelperFunctions.DevLogError(e.Message);
        }
    }

    private void RemoveAccessToken() {
        FileAccountManager.DeleteFile(FileAccountManager.ServerAccessToken);
    }

    #endregion

    private void SignUpSuccessCallBack() {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyRegistrationComplete);
        SaveLogInType(LogInType.Email);
    }

    private void LogInToServer(LogInType logInType) {

        if (_authController.IsNewUser())
            AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyRegistrationComplete);

        SaveLogInType(logInType);

        if (logInType == LogInType.Email && !_authController.IsVerifiried()) {
            CallBacks.onNeedVerification?.Invoke(_authController.GetEmail());
            return;
        }

        _authController.GetFirebaseToken((firebaseAccessToken) => GetServerAccessToken(firebaseAccessToken), CallBacks.onFail);
    }

    private void GetServerAccessToken(string firebaseAccessToken) {

        _onCancelLogIn?.InvokeAction();

        _onCancelLogIn = new ActionWrapper();

        if (!canLogIn) {
            //  CallBacks.onFail?.Invoke("Can't Get Server Access Token");
            return;
        }
        canLogIn = false;

        string url = GetRequestAccessTokenURL();

        FirebaseJsonToken firebaseJsonToken = new FirebaseJsonToken(firebaseAccessToken);

        _webRequestHandler.Post(url, firebaseJsonToken, WebRequestBodyType.JSON,
            (code, data) => { SaveAccessToken(data); SuccessRequestAccessTokenCallBack(code, data); },
            ErrorRequestAccessTokenCallBack, onCancel: _onCancelLogIn, needHeaderAccessToken: false);
    }

    private void SuccessRequestAccessTokenCallBack(long code, string data) {
        canLogIn = true;
        CallBacks.onSignInSuccess?.Invoke();
    }

    private void ErrorRequestAccessTokenCallBack(long code, string data) {
        HelperFunctions.DevLog("ErrorRequestAccessTokenCallBack " + code + " " + data);
        canLogIn = true;
        CallBacks.onFail?.Invoke(data);
    }

    private void QuickLogInWithDelay() {
        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task.Delay(QUICK_LOGIN_DELAY_TIME).ContinueWith((_) => QuickLogIn(), taskScheduler);
    }

    #region request urls
    private string GetRequestRefreshTokenURL() {
        return _webRequestHandler.ServerURLAuthAPI + _authorizationAPI.RefreshToken;
    }

    private string GetRequestAccessTokenURL() {
        return _webRequestHandler.ServerURLAuthAPI + _authorizationAPI.FirebaseToken;
    }

    #endregion

    private void OnEnable() {
        CallBacks.onFirebaseSignInSuccess += LogInToServer;
        CallBacks.onSignUpSuccess += SignUpSuccessCallBack;
    }

    private void OnDisable() {
        CallBacks.onFirebaseSignInSuccess -= LogInToServer;
        CallBacks.onSignUpSuccess -= SignUpSuccessCallBack;
    }

    private void OnDestroy() {
        CallBacks.onQuickLogInRequest -= QuickLogInWithDelay;
        CallBacks.onLogOutRequest -= LogOut;
    }
}