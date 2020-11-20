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

    public Action OnStartResetPassword;
    public Action<BadRequestStartResetPassword> OnErrorStartResetPassword;

    public Action OnResetPassword;
    public Action<BadRequestResetPassword> OnErrorResetPassword;

    public Action OnChangePassword;
    public Action<BadRequestChangePassword> OnErrorChangePassword;

    [SerializeField]
    AccountManager accountManager;
    [SerializeField]
    WebRequestHandler webRequestHandler;
    [SerializeField]
    AuthorizationAPIScriptableObject authorizationAPI;

    private string lastSignUpEmail;

    public void SignUp(EmailSignUpJsonData emailSignUpJsonData) {
        SignUpRequest(emailSignUpJsonData);
    }

    public void Verify(VerifyKeyJsonData verifyKeyJsonData) {
        VerifyRequest(verifyKeyJsonData);
    }

    public void LogIn(EmailLogInJsonData emailLogInJsonData) {
        LogInRequest(emailLogInJsonData);
    }

    public void StartResetPassword(ResetPasswordEmailJsonData resetPasswordEmailJsonData) {
        StartResetPasswordRequest(resetPasswordEmailJsonData);
    }

    public void ResetPassword(ResetPasswordJsonData resetPasswordJsonData) {
        ResetPasswordRequest(resetPasswordJsonData);
    }

    public void ChangePassword(PasswordChangeJsonData passwordChangeJsonData) {
        ChangePasswordRequest(passwordChangeJsonData);
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

        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyUserSignup, AnalyticParameters.ParamUserType, AnalyticParameters.ParamViewer); //TODO: update this to take account tye into account if users can register as broadcasters in the future
    }

    private void SignUpCallBack(long code, string body) {
        Debug.Log("SignUpCallBack " + code + " " + body);
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
        accountManager.SaveAccessToken(body);
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
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyUserLogin, AnalyticParameters.ParamUserType , accountManager.GetAccountType() == AccountManager.AccountType.Subscriber ? AnalyticParameters.ParamViewer:AnalyticParameters.ParamBroadcaster ); // Using keys in case enum changes names in future
        OnLogIn?.Invoke();
    }

    private void ErrorLogInCallBack(long code, string body) {
        BadRequestLogInEmailJsonData badRequestData = JsonUtility.FromJson<BadRequestLogInEmailJsonData>(body);
        OnErrorLogIn?.Invoke(badRequestData);
    }

    #endregion

    #region Reset Password
    private void StartResetPasswordRequest(ResetPasswordEmailJsonData resetPasswordEmailJsonData) {
        string url = GetRequestURL(authorizationAPI.ResetPassword);
        webRequestHandler.PostRequest(url, resetPasswordEmailJsonData, WebRequestHandler.BodyType.JSON,
            StartResetPasswordCallBack,
            ErrorStartResetPasswordCallBack);
    }

    private void StartResetPasswordCallBack(long code, string body) {
        Debug.Log("Start Reset Password " + code + " : " + body);
        OnStartResetPassword?.Invoke();
    }

    private void ErrorStartResetPasswordCallBack(long code, string body) {
        BadRequestStartResetPassword badRequestData = JsonUtility.FromJson<BadRequestStartResetPassword>(body);
        OnErrorStartResetPassword?.Invoke(badRequestData);
    }
    #endregion

    #region Reset Password verification
    private void ResetPasswordRequest(ResetPasswordJsonData resetPasswordJsonData) {
       string url = GetRequestURL(authorizationAPI.ResetPasswordConfirm);
       webRequestHandler.PostRequest(url, resetPasswordJsonData, WebRequestHandler.BodyType.JSON,
            ResetPasswordCallBack,
            ErrorResetPasswordCallBack);
    }

    private void ResetPasswordCallBack(long code, string body) {
        OnResetPassword?.Invoke();
    }

    private void ErrorResetPasswordCallBack(long code, string body) {
        BadRequestResetPassword badRequestResetPassword = JsonUtility.FromJson<BadRequestResetPassword>(body);
        OnErrorResetPassword?.Invoke(badRequestResetPassword);
    }
    #endregion

    #region Change Password 
    private void ChangePasswordRequest(PasswordChangeJsonData passwordChangeJsonData) {
        string url = GetRequestURL(authorizationAPI.ChangePassword);
        webRequestHandler.PostRequest(url, passwordChangeJsonData, WebRequestHandler.BodyType.JSON,
             ChangePasswordCallBack,
             ErrorChangePasswordCallBack,
             accountManager.GetAccessToken().access);
    }

    private void ChangePasswordCallBack(long code, string body) {
        OnChangePassword?.Invoke();
    }

    private void ErrorChangePasswordCallBack(long code, string body) {
        BadRequestChangePassword badRequestChangePassword = JsonUtility.FromJson<BadRequestChangePassword>(body);
        OnErrorChangePassword?.Invoke(badRequestChangePassword);
    }
    #endregion

    private string GetRequestURL(string postfix) {
        return webRequestHandler.ServerURLAuthAPI + postfix;
    }
}
