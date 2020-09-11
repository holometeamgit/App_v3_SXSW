using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class EmailAccountManager : MonoBehaviour
{
    public Action OnLogIn;
    public Action<BadRequestLogInEmailJsonData> OnErrorLogIn;

    [SerializeField]
    AccountManager accountManager;
    [SerializeField]
    WebRequestHandler webRequestHandler;
    [SerializeField]
    AuthorizationAPIScriptableObject authorizationAPI;

    [HideInInspector]
    public UnityEvent OnSignUp;
    [HideInInspector]
    public UnityEvent OnVerified;

    public void SignUp(EmailSignUpJsonData emailSignUpJsonData, ResponseDelegate responseCallBack, ErrorTypeDelegate errorCallBack) {
        SignUpRequest(emailSignUpJsonData, responseCallBack, errorCallBack);
    }

    public void Verify(VerifyKeyJsonData verifyKeyJsonData, ResponseDelegate responseCallBack, ErrorTypeDelegate errorCallBack) {
        VerifyRequest(verifyKeyJsonData, responseCallBack, errorCallBack);
    }

    public void LogIn(EmailLogInJsonData emailLogInJsonData) {
        LogInRequest(emailLogInJsonData);
    }

    private void Start() {
    }

    #region Sign Up

    private void SignUpRequest(EmailSignUpJsonData emailSignUpJsonData, ResponseDelegate responseCallBack, ErrorTypeDelegate errorCallBack) {
        string url = GetRequestURL(authorizationAPI.EmailSignUp);
        webRequestHandler.PostRequest(url, emailSignUpJsonData, WebRequestHandler.BodyType.JSON, responseCallBack, errorCallBack);
    }

    #endregion

    #region verification
    private void VerifyRequest(VerifyKeyJsonData verifyKeyJsonData, ResponseDelegate responseCallBack, ErrorTypeDelegate errorCallBack) {
        string url = GetRequestURL(authorizationAPI.EmailVerification);
        webRequestHandler.PostRequest(url, verifyKeyJsonData, WebRequestHandler.BodyType.JSON,
            (code, body) => {
                accountManager.SaveLastAutoType(LogInType.Email); responseCallBack.Invoke(code, body);
            },
            errorCallBack);
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
        OnLogIn.Invoke();
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
