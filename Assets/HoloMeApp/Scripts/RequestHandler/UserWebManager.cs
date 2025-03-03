﻿/*
 * The current script allows you to get and change user data.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using Beem.SSO;
using Beem.Firebase.DynamicLink;
using Zenject;

public class UserWebManager : MonoBehaviour {
    public Action OnLoadUserDataAfterLogIn;
    public Action OnUserInfoLoaded;
    public Action OnErrorUserInfoLoaded;

    public Action OnUserInfoUploaded;
    public Action<BadRequestUserUploadJsonData> OnErrorUserUploaded;

    public Action OnUserAccountDeleted;
    public Action UserAccountDisabled;

    [SerializeField] AuthorizationAPIScriptableObject authorizationAPI;

    private UserJsonData userData;
    private WebRequestHandler _webRequestHandler;
    private BusinessLogoController _businessLogoController;

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler) {
        _webRequestHandler = webRequestHandler;
    }

    public void LoadUserInfo() {
        _webRequestHandler.Get(GetRequestGetUserURL(), LoadUserInfoCallBack,
            ErrorLoadUserInfoCallBack, needHeaderAccessToken: true);
    }

    public void UploadUserInfo() {
        _webRequestHandler.Put(GetRequestPutUserURL(), userData,
            WebRequestBodyType.JSON, UploadUserInfoCallBack,
            ErrorUploadUserInfoCallBack, needHeaderAccessToken: true);
    }

    public void DeleteUserAccount() {
        _webRequestHandler.Delete(GetRequestDeleteUserURL(), DeleteUserInfoCallBack,
            ErrorMsgCallBack, needHeaderAccessToken: true);
    }

    public void DisableUserAccount() {
        _webRequestHandler.Delete(GetRequestDisableUserURL(), DisableUserInfoCallBack,
            ErrorMsgCallBack, needHeaderAccessToken: true);
    }

    public void UpdateUserData(string userName = null,
        string email = null,
        string first_name = null,
        string last_name = null,
        string bio = null,
        string profile_picture_s3_url = null) {

        LoadUserInfo(() =>
       UpdateUserDataAfterLoadUserInfo(userName, email, first_name, last_name,
           bio, profile_picture_s3_url));
    }

    public long GetUserID() {
        if (userData == null)
            return -1;
        return userData.pk;
    }

    /// <summary>
    /// GetCapabilities
    /// </summary>
    /// <returns></returns>
    public List<string> GetCapabilities() {
        return userData?.profile?.capabilities;
    }

    /* public string GetUnituniqueName() {
         return GetEmail();
     }*/

    /// <summary>
    /// Return Business profile name
    /// </summary>
    /// <returns>Business Profile Name</returns>
    public string GetBusinessProfileName() {
        return "Beem";
    }

    public bool IsBusinessProfile() {
        return true;
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

    public bool CanGoLive() {
        if (userData == null || userData.profile == null)
            return false;
        return userData.profile.go_live_feature;
    }

    public bool CanStartRoom() {
        if (userData == null || userData.profile == null)
            return false;
        return userData.profile.room_feature;
    }

    public void LoadUserInfo(Action loadUserInfoCallBack) {
        _webRequestHandler.Get(GetRequestGetUserURL(), (code, body) => loadUserInfoCallBack(),
            ErrorMsgCallBack, needHeaderAccessToken: true);
    }

    public string GetEmail() {
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

        UploadUserInfo();
    }

    #region download user data
    private void LoadUserInfoCallBack(long code, string body) {
        bool isLoadUserDataAfterLogIn = userData == null;
        try {
            userData = JsonUtility.FromJson<UserJsonData>(body);
            OnUserInfoLoaded?.Invoke();
            CallBacks.onUserDataLoaded?.Invoke();
        } catch (System.Exception e) {
            HelperFunctions.DevLogError(e.Message);
        }

        if (userData == null)
            userData = new UserJsonData();

        if (isLoadUserDataAfterLogIn) {
            OnLoadUserDataAfterLogIn?.Invoke();
        }
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
            if (code != 500) {
                badRequest = JsonUtility.FromJson<BadRequestUserUploadJsonData>(body);
            } else {
                badRequest = new BadRequestUserUploadJsonData();
                badRequest.code = code;
                badRequest.errorMsg = body;
            }

            OnErrorUserUploaded?.Invoke(badRequest);
        } catch (System.Exception) {
            badRequest = new BadRequestUserUploadJsonData();
            badRequest.code = code;
            badRequest.errorMsg = body;
            OnErrorUserUploaded?.Invoke(badRequest);
        }
    }

    #endregion

    private void RemoveUserData() {
        userData = null;
    }

    #region delete and disable user
    private void DeleteUserInfoCallBack(long code, string body) {
        OnUserAccountDeleted?.Invoke();
    }

    private void DisableUserInfoCallBack(long code, string body) {
        UserAccountDisabled?.Invoke();
    }
    #endregion

    private void ErrorMsgCallBack(long code, string body) {
    }


    #region url generation functions
    private string GetRequestGetUserURL() {
        return _webRequestHandler.ServerURLAuthAPI + authorizationAPI.GetUser;
    }

    private string GetRequestPutUserURL() {
        return _webRequestHandler.ServerURLAuthAPI + authorizationAPI.PutUser;
    }

    private string GetRequestDeleteUserURL() {
        return _webRequestHandler.ServerURLAuthAPI + authorizationAPI.DeleteUser;
    }

    private string GetRequestDisableUserURL() {
        return _webRequestHandler.ServerURLAuthAPI + authorizationAPI.DisableUser;
    }

    #endregion

    private void OnEnable() {
        CallBacks.onSignOut += RemoveUserData;
    }

    private void OnDisable() {
        CallBacks.onSignOut -= RemoveUserData;
    }
}
