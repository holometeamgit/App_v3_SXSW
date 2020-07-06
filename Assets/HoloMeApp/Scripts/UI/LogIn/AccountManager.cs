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
    public void SaveAccessToken(string serverAccessToken) {
        ServerAccessToken accessToken = JsonUtility.FromJson<ServerAccessToken>(serverAccessToken);
        FileAccountManager.SaveFile(nameof(FileAccountManager.ServerAccessToken), accessToken, FileAccountManager.ServerAccessToken);
    }

    public void LogIn(ResponseDelegate responseCallBack, ErrorTypeDelegate errorTypeCallBack) {
        Debug.Log("AccountManager LogIn");
        ServerAccessToken accessToken = LoadAccessToken();
        if(accessToken == null) {
            errorTypeCallBack.Invoke(0, "Server Access Token file doesn't exist");
            return;
        }

        Debug.Log(accessToken.access);

        webRequestHandler.PostRequest(GetRequestRefreshTokenURL(),
            accessToken, WebRequestHandler.BodyType.JSON,
            responseCallBack, errorTypeCallBack);
    }

    public void LogOut() {
        RemoveAccessToken();
        SaveLastAutoType(LogInType.None);
    }

    public void SaveLastAutoType(LogInType logInType) {
        PlayerPrefs.SetInt(nameof(PlayerPrefsKeys.LastTypeLoginPPKey), (int)logInType);
        PlayerPrefs.Save();
    }

    #endregion

    #region public account data

    public AccountType GetAccountType() {
        return accountType;
    }

    #endregion 

    private void RemoveAccessToken() {
        FileAccountManager.DeleteFile(FileAccountManager.ServerAccessToken);
    }

    private string GetRequestRefreshTokenURL() {
        return webRequestHandler.serverURLAuthAPI + getNewAccessTokenAPI;
    }

    private ServerAccessToken LoadAccessToken() {
        ServerAccessToken accessToken =
            FileAccountManager.ReadFile<ServerAccessToken>(nameof(FileAccountManager.ServerAccessToken),
            FileAccountManager.ServerAccessToken);
        return accessToken;
    }
}