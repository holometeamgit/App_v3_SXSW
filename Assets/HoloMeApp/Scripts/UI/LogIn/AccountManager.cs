using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Beem.SSO;

public class AccountManager : MonoBehaviour {
    public AuthController authController;

    [SerializeField]
    AuthorizationAPIScriptableObject authorizationAPI;
    [SerializeField]
    WebRequestHandler webRequestHandler;

    private Action onCancelLogIn;

    private bool canLogIn = true;

    #region public authorization

    public void CancelLogIn() {
        onCancelLogIn?.Invoke();
        HelperFunctions.DevLog("Cancel Log In");
    }

    public void QuickLogIn() {
        HelperFunctions.DevLog("try QuickLogIn");

        if (!canLogIn) {
            return;
        }
        canLogIn = false;
        if (GetLogInType() == LogInType.Facebook) {
            LogOut();
        }

        ServerAccessToken accessToken = GetAccessToken();

        if (accessToken == null && !authController.HasUser()) {
            ErrorRequestAccessTokenCallBack(0, "Server Access Token file doesn't exist");
            //errorTypeCallBack?.Invoke();
            return;
        } else if (accessToken == null && authController.HasUser() && GetLogInType() != LogInType.None) {
            HelperFunctions.DevLog("QuickLogIn Firebase");
            authController.DoAfterReloadUser(() => CallBacks.onFirebaseSignInSuccess(GetLogInType())); //TODO need test 
            return;
        } else if (accessToken == null && authController.HasUser() && GetLogInType() == LogInType.None) {
            LogOut();
            ErrorRequestAccessTokenCallBack(0, "Server Access Token file doesn't exist");
            return;
        }

        webRequestHandler.PostRequest(GetRequestRefreshTokenURL(),
            accessToken, WebRequestHandler.BodyType.JSON,
            (code, body) => { UpdateAccessToken(body); SuccessRequestAccessTokenCallBack(code, body); },
            ErrorRequestAccessTokenCallBack, onCancel: onCancelLogIn);
    }

    public void LogOut() {
        Debug.Log("LogOut");
        RemoveAccessToken();
        CallBacks.onSignOut?.Invoke();
        SaveLogInType(LogInType.None);
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
    public void SaveAccessToken(string serverAccessToken) {
        try {
            //            Debug.Log("Try Save Access Token \n" + serverAccessToken);
            ServerAccessToken accessToken = JsonUtility.FromJson<ServerAccessToken>(serverAccessToken);
            HelperFunctions.DevLog("serverAccessToken");
            HelperFunctions.DevLog(serverAccessToken);
            FileAccountManager.SaveFile(nameof(FileAccountManager.ServerAccessToken), accessToken, FileAccountManager.ServerAccessToken);
            //            Debug.Log("Access Token Saved");
        } catch (System.Exception e) { }
    }

    public ServerAccessToken GetAccessToken() {
        return FileAccountManager.ReadFile<ServerAccessToken>(nameof(FileAccountManager.ServerAccessToken),
            FileAccountManager.ServerAccessToken);
    }

    private void UpdateAccessToken(string serverAccessToken) {
        try {
            ServerAccessToken currentAccessToken = GetAccessToken();
            if (currentAccessToken == null)
                return;
            ServerAccessToken accessToken = JsonUtility.FromJson<ServerAccessToken>(serverAccessToken);
            currentAccessToken.access = accessToken.access;

            HelperFunctions.DevLog("serverAccessToken");
            HelperFunctions.DevLog(serverAccessToken);
            FileAccountManager.SaveFile(nameof(FileAccountManager.ServerAccessToken), currentAccessToken, FileAccountManager.ServerAccessToken);
        } catch (System.Exception e) { }
    }

    private void RemoveAccessToken() {
        FileAccountManager.DeleteFile(FileAccountManager.ServerAccessToken);
    }

    #endregion

    private void SignUpSuccessCallBack() {
        SaveLogInType(LogInType.Email);
    }

    private void LogInToServer(LogInType logInType) {
        SaveLogInType(logInType);

        HelperFunctions.DevLog("LogInToServer " + logInType + " IsVerifiried " + authController.IsVerifiried());

        if (logInType == LogInType.Email && !authController.IsVerifiried()) {
            CallBacks.onNeedVerification?.Invoke(authController.GetEmail());
            return;
        }

        authController.GetFirebaseToken((firebaseAccessToken) => GetServerAccessToken(firebaseAccessToken), CallBacks.onFail);
    }

    private void GetServerAccessToken(string firebaseAccessToken) {

        HelperFunctions.DevLog("GetServerAccessToken " + canLogIn);

        if (!canLogIn) {
          //  CallBacks.onFail?.Invoke("Can't Get Server Access Token");
            return;
        }
        canLogIn = false;

        string url = GetRequestAccessTokenURL();
        HelperFunctions.DevLog(url);

        FirebaseJsonToken firebaseJsonToken = new FirebaseJsonToken(firebaseAccessToken);

        webRequestHandler.PostRequest(url, firebaseJsonToken, WebRequestHandler.BodyType.JSON,
            (code, data) => SuccessRequestAccessTokenCallBack(code, data),
            ErrorRequestAccessTokenCallBack, onCancel: onCancelLogIn);
    }

    private void SuccessRequestAccessTokenCallBack(long code, string data) {
        HelperFunctions.DevLog("SuccessRequestAccessTokenCallBack " + code + " " + data);
        canLogIn = true;
        try {
            SaveAccessToken(data);
        } catch (System.Exception) { }
        CallBacks.onSignInSuccess?.Invoke();
    }

    private void ErrorRequestAccessTokenCallBack(long code, string data) {
        HelperFunctions.DevLogError("ErrorRequestAccessTokenCallBack " + code + " " + data);
        canLogIn = true;
        CallBacks.onFail?.Invoke(code + " : " + data);
    }

    private void Awake() {
        canLogIn = true;
    }

    private void OnEnable() {
        CallBacks.onFirebaseSignInSuccess += LogInToServer;
        CallBacks.onSignUpSuccess += SignUpSuccessCallBack;
    }

    private void OnDisable() {
        CallBacks.onFirebaseSignInSuccess -= LogInToServer;
        CallBacks.onSignUpSuccess -= SignUpSuccessCallBack;
    }

    #region request urls
    private string GetRequestRefreshTokenURL() {
        return webRequestHandler.ServerURLAuthAPI + authorizationAPI.RefreshToken;
    }

    private string GetRequestAccessTokenURL() {
        return webRequestHandler.ServerURLAuthAPI + authorizationAPI.FirebaseToken;
    }

    #endregion
}