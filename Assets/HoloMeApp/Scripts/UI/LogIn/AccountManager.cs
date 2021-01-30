using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Beem.SSO;

public class AccountManager : MonoBehaviour
{
    public AuthController authController;

    [SerializeField]
    AuthorizationAPIScriptableObject authorizationAPI;
    [SerializeField]
    WebRequestHandler webRequestHandler;

    #region public authorization

    public void QuickLogIn(ResponseDelegate responseCallBack, ErrorTypeDelegate errorTypeCallBack) {
        Debug.Log("QuickLogIn");

        ServerAccessToken accessToken = GetAccessToken();

        if(accessToken == null) {
            errorTypeCallBack.Invoke(0, "Server Access Token file doesn't exist");
            return;
        }

        Debug.Log("QuickLogIn refreshToken " + accessToken.refresh);
        Debug.Log("QuickLogIn accessToken " + accessToken.access);

        webRequestHandler.PostRequest(GetRequestRefreshTokenURL(),
            accessToken, WebRequestHandler.BodyType.JSON,
            (code, body) => { UpdateAccessToken(body); responseCallBack(code, body);},
            errorTypeCallBack);
    }

    public void LogOut() {
        RemoveAccessToken();
        SaveLogInType(LogInType.None);
    }


    #endregion

    #region Auth Type

    public void SaveLogInType(LogInType type) {
        Debug.Log("SaveLogInType " + type);

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
            Debug.Log("serverAccessToken");
            Debug.Log(serverAccessToken);
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

            Debug.Log("serverAccessToken");
            Debug.Log(serverAccessToken);
            FileAccountManager.SaveFile(nameof(FileAccountManager.ServerAccessToken), currentAccessToken, FileAccountManager.ServerAccessToken);
        } catch (System.Exception e) { }
    }

    private void RemoveAccessToken() {
        FileAccountManager.DeleteFile(FileAccountManager.ServerAccessToken);
    }

    #endregion


    private void LogInToServer(LogInType logInType) {
        authController.GetFirebaseToken((firebaseAccessToken) => GetServerAccessToken(firebaseAccessToken, logInType));
    }

    private void GetServerAccessToken(string firebaseAccessToken, LogInType logInType) {
        string url = GetRequestAccessTokenURL();
        Debug.Log(firebaseAccessToken);
        Debug.Log(url);

        FirebaseJsonToken firebaseJsonToken = new FirebaseJsonToken(firebaseAccessToken);

        webRequestHandler.PostRequest(url, firebaseJsonToken, WebRequestHandler.BodyType.JSON,
            (code, data) => SuccessRequestAccessTokenCallBack(code, data, logInType),
            ErrorRequestAccessTokenCallBack);
        //TODO not forgot about background 
    }

    private void SuccessRequestAccessTokenCallBack(long code, string data, LogInType logInType) {
        switch (code) {
            case 200:
                Debug.Log("Acceess token: \n" + data);
                SaveAccessToken(data);
                SaveLogInType(logInType);
                Debug.Log(logInType);
                CallBacks.onSignInSuccess.Invoke();
                break;
        }
    }

    private void ErrorRequestAccessTokenCallBack(long code, string data) {
        Debug.Log(code + " : " + data);
        CallBacks.onFail.Invoke(code + " : " + data);
    }

    private void OnEnable() {
        CallBacks.onFirebaseSignInSuccess += LogInToServer;
    }

    private void OnDisable() {
        CallBacks.onFirebaseSignInSuccess -= LogInToServer;
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