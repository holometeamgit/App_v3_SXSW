using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Facebook.Unity;
using Beem.SSO;

public class FacebookAccountManager : MonoBehaviour {
    [SerializeField]
    AccountManager accountManager;
    [SerializeField]
    WebRequestHandler webRequestHandler;

    [Space]
    [SerializeField]
    AuthorizationAPIScriptableObject authorizationAPI;
    [SerializeField]
    List<string> permissions = new List<string>() { "public_profile", "email" };

    [Space]//TODO change to signal when DI will add
    [SerializeField] GameObject backgroudGO;

    public UnityEvent OnAuthorized;

    private Facebook.Unity.AccessToken _FBAccessToken;
    private string _accessToken;

    public void SignUp() {
        if (!FB.IsInitialized)
            FBInit();
        StopAllCoroutines();
        backgroudGO.SetActive(true);
        FB.LogInWithReadPermissions(permissions, AuthCallback);
    }

    public void LogIn() { //TODO update after server will change (now the server has only sign up)
        SignUp();
    }

    public void LogInWithSavedAccessToken(ResponseDelegate responseCallBack, ErrorTypeDelegate errorCallBack) {
        var aToken = LoadAccessToken();
        if (aToken == null) {
            errorCallBack.Invoke(0, "Missing saved Facebook book token");
            return;
        }

        GetServerAccessToken(aToken, responseCallBack, errorCallBack);
    }

    public void LogOut() {
        DeleteAccessToken();
    }

    public void SaveAccessTokens() {
        if (_FBAccessToken == null)
            return;
        SaveAccessToken(_FBAccessToken);
        Debug.Log("Facebook AccessToken save");
        _FBAccessToken = null;

        accountManager.SaveAccessToken(_accessToken);
        Debug.Log("Save Acceess token: \n" + _accessToken);
        _accessToken = null;
    }

    void Awake() {
        OnAuthorized.AddListener(() => AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyRegistrationComplete));
        FBInit();
    }

    #region Facebook initialization
    private void FBInit() {
        if (!FB.IsInitialized) {
            FB.Init(InitCallback, OnHideUnity);
        } else {
            FB.ActivateApp();
        }
    }

    private void InitCallback() {
        if (FB.IsInitialized) {
            FB.ActivateApp();
        } else {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown) {
        if (!isGameShown) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }
    }
    #endregion

    #region Facebook Sign Up
    private void AuthCallback(ILoginResult result) {
        if (FB.IsLoggedIn) {
            _FBAccessToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            Debug.Log("Facebook Sign Up");
            GetServerAccessToken(_FBAccessToken, SuccessRequestAccessTokenCallBack, ErrorRequestAccessTokenCallBack);
        } else {
            Debug.Log("User cancelled login");
            backgroudGO.SetActive(false);
        }
    }

    private void ErrorRequestAccessTokenCallBack(long code, string data) {
        Debug.Log(code + " : " + data);
        backgroudGO.SetActive(false);
    }

    private void SuccessRequestAccessTokenCallBack(long code, string data) {
        switch (code) {
            case 200:
                Debug.Log("Acceess token: \n" + data);
                _accessToken = data;
                SaveAccessTokens();
                OnAuthorized.Invoke();
                backgroudGO.SetActive(false);
                break;
        }
    }
    #endregion

    #region request server access token
    private void GetServerAccessToken(Facebook.Unity.AccessToken accessToken, ResponseDelegate responseCallBack, ErrorTypeDelegate errorCallBack) {
        string url = GetRequestAccessTokenURL(accessToken.TokenString);
        webRequestHandler.GetRequest(url, responseCallBack, errorCallBack);
    }

    private string GetStringPermissions() {
        string resultString = "";
        for (int i = 0; i < permissions.Count; i++) {
            resultString += permissions[i];
            if (i < permissions.Count - 1)
                resultString += ",";
        }
        return resultString;
    }

    private string GetRequestAccessTokenURL(string FBAccessToken) {
        return webRequestHandler.ServerURLAuthAPI + authorizationAPI.FacebookCompliteLogIn +
            "?code=" + FBAccessToken +
            "&state" + GetStringPermissions();
    }
    #endregion

    #region save/load/delete access token file
    private void SaveAccessToken(Facebook.Unity.AccessToken accessToken) {
        FileAccountManager.SaveFile(nameof(FileAccountManager.FacebookTokenFileName), accessToken, FileAccountManager.FacebookTokenFileName);

        accountManager.SaveLogInType(LogInType.Facebook);
    }

    private Facebook.Unity.AccessToken LoadAccessToken() {
        var aToken = FileAccountManager.ReadFile<Facebook.Unity.AccessToken>(nameof(FileAccountManager.FacebookTokenFileName), FileAccountManager.FacebookTokenFileName);
        return aToken;
    }

    private void DeleteAccessToken() {
        FileAccountManager.DeleteFile(FileAccountManager.FacebookTokenFileName);
    }
    #endregion


}