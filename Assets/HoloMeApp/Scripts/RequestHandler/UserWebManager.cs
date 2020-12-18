/*
 * The current script allows you to get and change user data.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class UserWebManager : MonoBehaviour
{
    public Action OnUserInfoLoaded;
    public Action OnErrorUserInfoLoaded;

    public Action OnUserInfoUploaded;
    public Action<BadRequestUserUploadJsonData> OnErrorUserUploaded;

    public Action OnUserAccountDeleted;
    public Action UserAccountDisabled;

    [SerializeField] WebRequestHandler webRequestHandler;
    [SerializeField] AccountManager accountManager;
    [SerializeField] AuthorizationAPIScriptableObject authorizationAPI;

    private UserJsonData userData;

    [HideInInspector]


    public void LoadUserInfo() {
        webRequestHandler.GetRequest(GetRequestGetUserURL(), LoadUserInfoCallBack,
            ErrorLoadUserInfoCallBack, accountManager.GetAccessToken().access);
    }

    public void UploadUserInfo() {
        webRequestHandler.PutRequest(GetRequestPutUserURL(), userData,
            WebRequestHandler.BodyType.JSON, UploadUserInfoCallBack,
            ErrorUploadUserInfoCallBack, accountManager.GetAccessToken().access);
    }

    public void DeleteUserAccount() {
        webRequestHandler.DeleteRequest(GetRequestDeleteUserURL(), DeleteUserInfoCallBack,
            ErrorMsgCallBack, accountManager.GetAccessToken().access);
    }

    public void DisableUserAccount() {
        webRequestHandler.DeleteRequest(GetRequestDisableUserURL(), DisableUserInfoCallBack,
            ErrorMsgCallBack, accountManager.GetAccessToken().access);
    }

    public void UpdateUserData(string userName = null,
        string email = null,
        string first_name = null,
        string last_name = null,
        string bio = null,
        string profile_picture_s3_url = null) {

        Debug.Log(userName + " " +
            email + " " +
            first_name + " " +
            last_name + " " +
            bio + " " +
            profile_picture_s3_url);

        LoadUserInfo( () =>
        UpdateUserDataAfterLoadUserInfo(userName, email, first_name, last_name,
            bio, profile_picture_s3_url));
    }

    public long GetUserID()
    {
        if (userData == null)
            return -1;
        return userData.pk;
    }

    public string GetUnituniqueName() {
        return GetEmail();
    }

    public string GetFullName() {
        if (userData == null || string.IsNullOrEmpty(userData.first_name))
            return null;
        return userData.first_name + " " + userData.last_name;
    }

    public string GetFirstName() {
        if (userData == null || string.IsNullOrEmpty(userData.first_name))
            return null;
        return userData.first_name;
    }

    public string GetLastName() {
        if (userData == null || string.IsNullOrEmpty(userData.first_name))
            return null;
        return userData.last_name;
    }

    public string GetUsername() {
        if (userData == null || string.IsNullOrEmpty(userData.username))
            return null;
        return userData.username;
    }

    public string GetBio() {
        if (userData == null || userData.profile == null || string.IsNullOrEmpty(userData.profile.bio))
            return null;
        return userData.profile.bio;
    }

    public bool IsBroadcaster() {
        if (userData == null || userData.profile == null)
            return false;
        return userData.profile.is_creator;
    }

    public void LoadUserInfo(Action loadUserInfoCallBack) {
        webRequestHandler.GetRequest(GetRequestGetUserURL(), (code, body) => loadUserInfoCallBack(),
            ErrorMsgCallBack, accountManager.GetAccessToken().access);
    }

    private string GetEmail() {
        if (userData == null || string.IsNullOrEmpty(userData.email))
            return null;
        return userData.email;
    }

    private void UpdateUserDataAfterLoadUserInfo(string userName, string email,
        string first_name, string last_name, string bio, string profile_picture_s3_url) {
        if (userData == null)
            userData = new UserJsonData();

        userData.username = userName ?? userData.username;
        userData.email = email ?? userData.email;
        userData.first_name = first_name ?? userData.first_name;
        userData.last_name = last_name ?? userData.last_name;

        userData.profile = userData.profile ?? new ProfileJsonData();
        userData.profile.bio = bio ?? userData.profile.bio;
        userData.profile.profile_picture_s3_url = profile_picture_s3_url ?? userData.profile.profile_picture_s3_url;

        Debug.Log(userName + " " +
            email + " " +
            first_name + " " +
            last_name + " " +
            bio + " " +
            profile_picture_s3_url);

        UploadUserInfo();
    }

    #region download user data
    private void LoadUserInfoCallBack(long code, string body) {
        try {
            userData = JsonUtility.FromJson<UserJsonData>(body);
            OnUserInfoLoaded?.Invoke();
        } catch (System.Exception e) { }

        if (userData == null)
            userData = new UserJsonData();
    }

    private void ErrorLoadUserInfoCallBack(long code, string body) {
        OnErrorUserInfoLoaded?.Invoke();
    }
    #endregion

    #region upload user
    private void UploadUserInfoCallBack(long code, string body) {
        OnUserInfoUploaded?.Invoke();
    }

    private void ErrorUploadUserInfoCallBack(long code, string body) {
        BadRequestUserUploadJsonData badRequest;
        try {
            Debug.Log("ErrorUploadUserInfoCallBack " + code + " " + body);
            badRequest = JsonUtility.FromJson<BadRequestUserUploadJsonData>(body);

            OnErrorUserUploaded?.Invoke(badRequest);
        } catch (System.Exception) {
            badRequest = new BadRequestUserUploadJsonData();
            badRequest.code = code;
            badRequest.errorMsg = body;
        }
    }

    #endregion

    #region delete and disable user
    private void DeleteUserInfoCallBack(long code, string body) {
        Debug.Log("DeleteUserInfoCallBack " + code + " " + body);
        OnUserAccountDeleted?.Invoke();
    }

    private void DisableUserInfoCallBack(long code, string body) {
        UserAccountDisabled?.Invoke();
    }
    #endregion

    private void ErrorMsgCallBack(long code, string body) {
        Debug.LogWarning(code + " " + body);
    }
 

    #region url generation functions
    private string GetRequestGetUserURL() {
        return webRequestHandler.ServerURLAuthAPI + authorizationAPI.GetUser;
    }

    private string GetRequestPutUserURL() {
        return webRequestHandler.ServerURLAuthAPI + authorizationAPI.PutUser;
    }

    private string GetRequestDeleteUserURL() {
        return webRequestHandler.ServerURLAuthAPI + authorizationAPI.DeleteUser;
    }

    private string GetRequestDisableUserURL() {
        return webRequestHandler.ServerURLAuthAPI + authorizationAPI.DisableUser;
    }

    #endregion
}
