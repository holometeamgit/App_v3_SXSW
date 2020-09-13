using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class EmailAccountManager : MonoBehaviour
{
    public Action OnSignUp;
    public Action<BadRequestSignUpEmailJsonData> OnErrorSignUp;

    public Action OnLogIn;
    public Action<BadRequestLogInEmailJsonData> OnErrorLogIn;

    public Action OnVerified;
    public Action OnErrorVerification;

    [SerializeField]
    AccountManager accountManager;
    [SerializeField]
    WebRequestHandler webRequestHandler;
    [SerializeField]
    AuthorizationAPIScriptableObject authorizationAPI;

    private string lastSignUpEmail;
    private string signUpAccessToken;

    public void SignUp(EmailSignUpJsonData emailSignUpJsonData) {
        SignUpRequest(emailSignUpJsonData);
    }

    public void Verify(VerifyKeyJsonData verifyKeyJsonData) {
        VerifyRequest(verifyKeyJsonData);
    }

    public void LogIn(EmailLogInJsonData emailLogInJsonData) {
        LogInRequest(emailLogInJsonData);
    }

    public string GetLastSignUpEmail() {
        return lastSignUpEmail;
    }

    #region Sign Up

    private void SignUpRequest(EmailSignUpJsonData emailSignUpJsonData) {
        string url = GetRequestURL(authorizationAPI.EmailSignUp);
        lastSignUpEmail = emailSignUpJsonData.email;
        webRequestHandler.PostRequest(url, emailSignUpJsonData, WebRequestHandler.BodyType.JSON,
            SignUpCallBack,
            ErrorSignUpCallBack);
    }

    private void SignUpCallBack(long code, string body) {
        Debug.Log("SignUpCallBack " + code + " " + body);
        signUpAccessToken = body;
        OnSignUp?.Invoke();
    }

    private void ErrorSignUpCallBack(long code, string body) {
        BadRequestSignUpEmailJsonData badRequestData = JsonUtility.FromJson<BadRequestSignUpEmailJsonData>(body);
        Debug.Log("ErrorSignUpCallBack " + code + " " + body);
        OnErrorSignUp?.Invoke(badRequestData);
    }

    #endregion

    #region verification
    private void VerifyRequest(VerifyKeyJsonData verifyKeyJsonData) {
        string url = GetRequestURL(authorizationAPI.EmailVerification);
        webRequestHandler.PostRequest(url, verifyKeyJsonData, WebRequestHandler.BodyType.JSON,
            VerifedCallBack,
            ErrorVerifyCallBack);
    }

    private void VerifedCallBack(long code, string body) {
        accountManager.SaveAccessToken(signUpAccessToken);
        signUpAccessToken = "";
        accountManager.SaveLastAutoType(LogInType.Email);
        OnVerified?.Invoke();
    }

    private void ErrorVerifyCallBack(long code, string body) {
        OnErrorVerification?.Invoke();
    }

    #endregion

    #region Log In
    private void LogInRequest(EmailLogInJsonData emailLogInJsonData) {
        string url = GetRequestURL(authorizationAPI.EmailLogIn);
        webRequestHandler.PostRequest(url, emailLogInJsonData, WebRequestHandler.BodyType.JSON,
            LogInCallBack,
            ErrorLogInCallBack);
    }

    private void LogInCallBack(long code, string body) {
        Debug.Log("Log In " + code + " : " + body);
        accountManager.SaveLastAutoType(LogInType.Email);
        accountManager.SaveAccessToken(body);
        OnLogIn?.Invoke();
    }

    private void ErrorLogInCallBack(long code, string body) {
        BadRequestLogInEmailJsonData badRequestData = JsonUtility.FromJson<BadRequestLogInEmailJsonData>(body);
        OnErrorLogIn?.Invoke(badRequestData);
    }

    #endregion

    private string GetRequestURL(string postfix) {
        return webRequestHandler.ServerURLAuthAPI + postfix;
    }
}
