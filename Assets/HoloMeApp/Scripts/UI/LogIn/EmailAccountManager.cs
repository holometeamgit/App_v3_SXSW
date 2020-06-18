using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class EmailAccountManager : MonoBehaviour
{
    [SerializeField]
    AccountManager accountManager;
    [SerializeField]
    WebRequestHandler webRequestHandler;

    [Header("API")]
    [SerializeField]
    string signUpAPI = "/signup/";
    [SerializeField]
    string verifyEmailAPI = "/signup/verify-email/";
    [SerializeField]
    string logInAPI = "/token/";

    public UnityEvent OnSignUp;
    public UnityEvent OnVerified;
    public UnityEvent OnLogIn;

    public void SignUp(EmailSignUpJsonData emailSignUpJsonData, ResponseDelegate responseCallBack, ErrorTypeDelegate errorCallBack) {
        SignUpRequest(emailSignUpJsonData, responseCallBack, errorCallBack);
    }

    public void Verify(VerifyKeyJsonData verifyKeyJsonData, ResponseDelegate responseCallBack, ErrorTypeDelegate errorCallBack) {
        VerifyRequest(verifyKeyJsonData, responseCallBack, errorCallBack);
    }

    public void LogIn(EmailLogInJsonData emailLogInJsonData, ResponseDelegate responseCallBack, ErrorTypeDelegate errorCallBack) {
        LogInRequest(emailLogInJsonData, responseCallBack, errorCallBack);
    }

    #region Sign Up

    private void SignUpRequest(EmailSignUpJsonData emailSignUpJsonData, ResponseDelegate responseCallBack, ErrorTypeDelegate errorCallBack) {
        string url = GetRequestURL(signUpAPI);
        webRequestHandler.PostRequest(url, emailSignUpJsonData, WebRequestHandler.BodyType.JSON, responseCallBack, errorCallBack);
    }

    #endregion

    #region verification
    private void VerifyRequest(VerifyKeyJsonData verifyKeyJsonData, ResponseDelegate responseCallBack, ErrorTypeDelegate errorCallBack) {
        string url = GetRequestURL(verifyEmailAPI);
        webRequestHandler.PostRequest(url, verifyKeyJsonData, WebRequestHandler.BodyType.JSON, responseCallBack, errorCallBack);
    }

    #endregion

    #region Log In
    private void LogInRequest(EmailLogInJsonData emailLogInJsonData, ResponseDelegate responseCallBack, ErrorTypeDelegate errorCallBack) {
        string url = GetRequestURL(logInAPI);
        webRequestHandler.PostRequest(url, emailLogInJsonData, WebRequestHandler.BodyType.JSON, responseCallBack, errorCallBack);
    }

    #endregion

    private string GetRequestURL(string postfix) {
        return webRequestHandler.serverURLAuthAPI + postfix;
    }
}
