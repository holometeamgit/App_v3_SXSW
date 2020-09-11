using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountManager : MonoBehaviour
{
    public enum AccountType {
        Broadcater,
        Subscriber
    }

    [SerializeField]
    string getNewAccessTokenAPI = "/token/refresh/";

    [SerializeField] WebRequestHandler webRequestHandler;

    [SerializeField] private AccountType accountType;

    #region public authorization

    public void LogIn(ResponseDelegate responseCallBack, ErrorTypeDelegate errorTypeCallBack) {
        Debug.Log("AccountManager LogIn");
        ServerAccessToken accessToken = LoadAccessToken();

        if(accessToken == null) {
            errorTypeCallBack.Invoke(0, "Server Access Token file doesn't exist");
            return;
        }

        Debug.Log("LogIn " + accessToken.refresh + "\n\n" + accessToken.access);

        webRequestHandler.PostRequest(GetRequestRefreshTokenURL(),
            accessToken, WebRequestHandler.BodyType.JSON,
            (code, body) => { UpdateAccessToke(body, accessToken); responseCallBack(code, body);},
            errorTypeCallBack);
    }

    public void LogOut() {
        Debug.Log("LogOut");
        RemoveAccessToken();
        SaveLastAutoType(LogInType.None);
    }

    public void SaveLastAutoType(LogInType logInType) {
        Debug.Log("SaveLastAutoType " + logInType.ToString());
        PlayerPrefs.SetInt(PlayerPrefsKeys.LastTypeLoginPPKey, (int)logInType);
        PlayerPrefs.Save();
    }

    public ServerAccessToken GetAccessToken() {
        return FileAccountManager.ReadFile<ServerAccessToken>(nameof(FileAccountManager.ServerAccessToken), FileAccountManager.ServerAccessToken);
    }

    public LogInType GetLoginType() {
        if (!PlayerPrefs.HasKey(PlayerPrefsKeys.LastTypeLoginPPKey)) {
            return LogInType.None;
        }
        return (LogInType)PlayerPrefs.GetInt(PlayerPrefsKeys.LastTypeLoginPPKey);
    }

    #endregion

    #region public account data

    public AccountType GetAccountType() {
        return accountType;
    }

    #endregion

    public void SaveAccessToken(string serverAccessToken) {
        try {
//            Debug.Log("Try Save Access Token \n" + serverAccessToken);
            ServerAccessToken accessToken = JsonUtility.FromJson<ServerAccessToken>(serverAccessToken);
            FileAccountManager.SaveFile(nameof(FileAccountManager.ServerAccessToken), accessToken, FileAccountManager.ServerAccessToken);
//            Debug.Log("Access Token Saved");
        } catch (System.Exception e) { }

        
    }

    private void UpdateAccessToke(string onlyAccess, ServerAccessToken accessToken) {
        var access = JsonUtility.FromJson<ServerAccessToken>(onlyAccess);
        Debug.Log("UpdatedAccessToke " + access.access);
        accessToken.access = access.access;
        SaveAccessToken(JsonUtility.ToJson(accessToken));
    }

    private void RemoveAccessToken() {
        FileAccountManager.DeleteFile(FileAccountManager.ServerAccessToken);
    }

    private string GetRequestRefreshTokenURL() {
        return webRequestHandler.ServerURLAuthAPI + getNewAccessTokenAPI;
    }

    private ServerAccessToken LoadAccessToken() {
        ServerAccessToken accessToken =
            FileAccountManager.ReadFile<ServerAccessToken>(nameof(FileAccountManager.ServerAccessToken),
            FileAccountManager.ServerAccessToken);
        return accessToken;
    }
}