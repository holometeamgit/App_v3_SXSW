/*
 * The current script allows you to get and change user data.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UserWebManager : MonoBehaviour
{
    [SerializeField] WebRequestHandler webRequestHandler;
    [SerializeField] AccountManager accountManager;
    [SerializeField] AuthorizationAPIScriptableObject authorizationAPI;

    private UserJsonData userData;

    [HideInInspector]
    public UnityEvent UserInfoUpdated;
    [HideInInspector]
    public UnityEvent UserInfoUploaded;
    [HideInInspector]
    public UnityEvent UserAccountDeleted;
    [HideInInspector]
    public UnityEvent UserAccountDisabled;

    public void UpdateUserInfo() {
        webRequestHandler.GetRequest(GetRequestGetUserURL(), UpdateUserInfoCallBack,
            ErrorMsgCallBack, accountManager.GetAccessToken().access);
    }

    public void UploadUserInfo() {
        webRequestHandler.PutRequest(GetRequestPutUserURL(), userData,
            WebRequestHandler.BodyType.JSON, UploadUserInfoCallBack,
            ErrorMsgCallBack, accountManager.GetAccessToken().access);
    }

    public void DeleteUserAccount() {
        webRequestHandler.DeleteRequest(GetRequestDeleteUserURL(), DeleteUserInfoCallBack,
            ErrorMsgCallBack, accountManager.GetAccessToken().access);
    }

    public void DisableUserAccount() {
        webRequestHandler.DeleteRequest(GetRequestDisableUserURL(), DisableUserInfoCallBack,
            ErrorMsgCallBack, accountManager.GetAccessToken().access);
    }

    #region call back function
    private void UpdateUserInfoCallBack(long code, string body) {
        try {
            userData = JsonUtility.FromJson<UserJsonData>(body);

            UserInfoUpdated.Invoke();
        } catch (System.Exception e) { }
    }

    private void UploadUserInfoCallBack(long code, string body) {
        UserInfoUploaded.Invoke();
    }

    private void DeleteUserInfoCallBack(long code, string body) {
        UserAccountDeleted.Invoke();
    }

    private void DisableUserInfoCallBack(long code, string body) {
        UserAccountDisabled.Invoke();
    }

    private void ErrorMsgCallBack(long code, string body) {
        Debug.LogWarning(code + " " + body);
    }
    #endregion

    #region url generation functions
    private string GetRequestGetUserURL() {
        return webRequestHandler.serverURLAuthAPI + authorizationAPI.GetUser;
    }

    private string GetRequestPutUserURL() {
        return webRequestHandler.serverURLAuthAPI + authorizationAPI.PutUser;
    }

    private string GetRequestDeleteUserURL() {
        return webRequestHandler.serverURLAuthAPI + authorizationAPI.DeleteUser;
    }

    private string GetRequestDisableUserURL() {
        return webRequestHandler.serverURLAuthAPI + authorizationAPI.DisableUser;
    }

    #endregion
}
