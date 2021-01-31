﻿using System.Collections;
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

    #region public authorization

    public void QuickLogIn(ResponseDelegate responseCallBack, ErrorTypeDelegate errorTypeCallBack) {

        ServerAccessToken accessToken = GetAccessToken();

        if (accessToken == null && !authController.IsVerifiried()) {
            errorTypeCallBack?.Invoke(0, "Server Access Token file doesn't exist");
            return;
        } else if (accessToken == null && authController.IsVerifiried() && GetLogInType() != LogInType.None) {
            CallBacks.onFirebaseSignInSuccess(GetLogInType()); //TODO need test 
            return;
        }

        Debug.Log("QuickLogIn refreshToken " + accessToken.refresh);
        Debug.Log("QuickLogIn accessToken " + accessToken.access);

        webRequestHandler.PostRequest(GetRequestRefreshTokenURL(),
            accessToken, WebRequestHandler.BodyType.JSON,
            (code, body) => { UpdateAccessToken(body); responseCallBack(code, body); },
            errorTypeCallBack);
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

    private void SignUpSuccessCallBack() {
        SaveLogInType(LogInType.Email);
    }

    private void LogInToServer(LogInType logInType) {
        SaveLogInType(logInType);

        if (logInType == LogInType.Email && !authController.IsVerifiried()) {
            CallBacks.onNeedVerification?.Invoke(authController.GetEmail());
            return;
        }

        authController.GetFirebaseToken((firebaseAccessToken) => GetServerAccessToken(firebaseAccessToken));
    }

    private void GetServerAccessToken(string firebaseAccessToken) {
        string url = GetRequestAccessTokenURL();
        Debug.Log(firebaseAccessToken);
        Debug.Log(url);

        FirebaseJsonToken firebaseJsonToken = new FirebaseJsonToken(firebaseAccessToken);

        webRequestHandler.PostRequest(url, firebaseJsonToken, WebRequestHandler.BodyType.JSON,
            (code, data) => SuccessRequestAccessTokenCallBack(code, data),
            ErrorRequestAccessTokenCallBack);
        //TODO not forgot about background 
    }

    private void SuccessRequestAccessTokenCallBack(long code, string data) {
        switch (code) {
            case 200:
                Debug.Log("Acceess token: \n" + data);
                SaveAccessToken(data);
                CallBacks.onSignInSuccess?.Invoke();
                break;
        }
    }

    private void ErrorRequestAccessTokenCallBack(long code, string data) {
        Debug.Log(code + " : " + data);
        CallBacks.onFail?.Invoke(code + " : " + data);
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