using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using Beem.SSO;
using Zenject;

public class EmailAccountManager : MonoBehaviour {
    public Action OnSignUp;
    public Action<BadRequestSignUpEmailJsonData> OnErrorSignUp;

    public Action OnResendVerification;
    public Action<BadRequestResendVerificationJsonData> OnErrorResendVerification;

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
    AuthorizationAPIScriptableObject authorizationAPI;

    private WebRequestHandler _webRequestHandler;
    private AccountManager _accountManager;

    private string lastSignUpEmail;

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler, AccountManager accountManager) {
        _webRequestHandler = webRequestHandler;
        _accountManager = accountManager;
    }

    public void SignUp(EmailSignUpJsonData emailSignUpJsonData) {
        SignUpRequest(emailSignUpJsonData);
    }

    public void Verify(VerifyKeyJsonData verifyKeyJsonData) {
        VerifyRequest(verifyKeyJsonData);
    }

    public void ResendVerification(ResendVerifyJsonData resendVerifyJsonData) {
        ResentVerificationRequest(resendVerifyJsonData);
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
        _webRequestHandler.Post(url, emailSignUpJsonData, WebRequestBodyType.JSON,
            SignUpCallBack,
            ErrorSignUpCallBack, needHeaderAccessToken: false);

        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyUserSignup, AnalyticParameters.ParamUserType, AnalyticParameters.ParamViewer); //TODO: update this to take account tye into account if users can register as broadcasters in the future
    }

    private void SignUpCallBack(long code, string body) {
        OnSignUp?.Invoke();
    }

    private void ErrorSignUpCallBack(long code, string body) {
        BadRequestSignUpEmailJsonData badRequestData;
        try {
            badRequestData = JsonUtility.FromJson<BadRequestSignUpEmailJsonData>(body);
        } catch (System.Exception) {
            badRequestData = new BadRequestSignUpEmailJsonData();
            badRequestData.code = code;
            badRequestData.errorMsg = body;
            OnErrorSignUp?.Invoke(badRequestData);
            return;
        }
        OnErrorSignUp?.Invoke(badRequestData);
    }
    #endregion

    #region Resend Verification

    private void ResentVerificationRequest(ResendVerifyJsonData resendVerifyJsonData) {
        string url = GetRequestURL(authorizationAPI.ResendVerification);
        lastSignUpEmail = resendVerifyJsonData.email;
        _webRequestHandler.Post(url, resendVerifyJsonData, WebRequestBodyType.JSON,
            ResentVerificationCallBack,
            ErrorResentVerificationBack, needHeaderAccessToken: false);
    }

    private void ResentVerificationCallBack(long code, string body) {
        OnResendVerification?.Invoke();
    }

    private void ErrorResentVerificationBack(long code, string body) {
        BadRequestResendVerificationJsonData badRequestData;
        try {
            badRequestData = JsonUtility.FromJson<BadRequestResendVerificationJsonData>(body);
        } catch (System.Exception) {
            badRequestData = new BadRequestResendVerificationJsonData();
            badRequestData.code = code;
            badRequestData.errorMsg = body;
            OnErrorResendVerification?.Invoke(badRequestData);
            return;
        }
        OnErrorResendVerification?.Invoke(badRequestData);
    }

    #endregion

    #region verification
    private void VerifyRequest(VerifyKeyJsonData verifyKeyJsonData) {
        string url = GetRequestURL(authorizationAPI.EmailVerification);
        _webRequestHandler.Post(url, verifyKeyJsonData, WebRequestBodyType.JSON,
            VerifedCallBack,
            ErrorVerifyCallBack, needHeaderAccessToken: false);
    }

    private void VerifedCallBack(long code, string body) {
        _accountManager.SaveAccessToken(body);
        _accountManager.SaveLogInType(LogInType.Email);
        OnVerified?.Invoke();
    }

    private void ErrorVerifyCallBack(long code, string body) {
        OnErrorVerification?.Invoke();
    }

    #endregion

    #region Log In
    private void LogInRequest(EmailLogInJsonData emailLogInJsonData) {
        string url = GetRequestURL(authorizationAPI.EmailLogIn);
        _webRequestHandler.Post(url, emailLogInJsonData, WebRequestBodyType.JSON,
            LogInCallBack,
            ErrorLogInCallBack, needHeaderAccessToken: false);
    }

    private void LogInCallBack(long code, string body) {
        _accountManager.SaveLogInType(LogInType.Email);
        _accountManager.SaveAccessToken(body);
        OnLogIn?.Invoke();
    }

    private void ErrorLogInCallBack(long code, string body) {
        BadRequestLogInEmailJsonData badRequestData;
        try {
            badRequestData = JsonUtility.FromJson<BadRequestLogInEmailJsonData>(body);
        } catch (System.Exception) {
            badRequestData = new BadRequestLogInEmailJsonData();
            badRequestData.code = code;
            badRequestData.errorMsg = body;
            OnErrorLogIn?.Invoke(badRequestData);
            return;
        }
        OnErrorLogIn?.Invoke(badRequestData);
    }

    #endregion

    #region Reset Password
    private void StartResetPasswordRequest(ResetPasswordEmailJsonData resetPasswordEmailJsonData) {
        string url = GetRequestURL(authorizationAPI.ResetPassword);
        _webRequestHandler.Post(url, resetPasswordEmailJsonData, WebRequestBodyType.JSON,
            StartResetPasswordCallBack,
            ErrorStartResetPasswordCallBack, needHeaderAccessToken: false);
    }

    private void StartResetPasswordCallBack(long code, string body) {
        OnStartResetPassword?.Invoke();
    }

    private void ErrorStartResetPasswordCallBack(long code, string body) {
        BadRequestStartResetPassword badRequestData;
        try {
            badRequestData = JsonUtility.FromJson<BadRequestStartResetPassword>(body);
        } catch (System.Exception) {
            badRequestData = new BadRequestStartResetPassword();
            badRequestData.code = code;
            badRequestData.errorMsg = body;
            OnErrorStartResetPassword?.Invoke(badRequestData);
            return;
        }

        if (badRequestData == null) {
            badRequestData = new BadRequestStartResetPassword();
            badRequestData.code = 500;
        }

        OnErrorStartResetPassword?.Invoke(badRequestData);
    }
    #endregion

    #region Reset Password verification
    private void ResetPasswordRequest(ResetPasswordJsonData resetPasswordJsonData) {
        string url = GetRequestURL(authorizationAPI.ResetPasswordConfirm);
        _webRequestHandler.Post(url, resetPasswordJsonData, WebRequestBodyType.JSON,
             ResetPasswordCallBack,
             ErrorResetPasswordCallBack, needHeaderAccessToken: false);
    }

    private void ResetPasswordCallBack(long code, string body) {
        OnResetPassword?.Invoke();
    }

    private void ErrorResetPasswordCallBack(long code, string body) {
        BadRequestResetPassword badRequestData;
        try {
            badRequestData = JsonUtility.FromJson<BadRequestResetPassword>(body);
        } catch (System.Exception) {
            badRequestData = new BadRequestResetPassword();
            badRequestData.code = code;
            badRequestData.errorMsg = body;
            OnErrorResetPassword?.Invoke(badRequestData);
            return;
        }
        OnErrorResetPassword?.Invoke(badRequestData);
    }
    #endregion

    #region Change Password 
    private void ChangePasswordRequest(PasswordChangeJsonData passwordChangeJsonData) {
        string url = GetRequestURL(authorizationAPI.ChangePassword);
        _webRequestHandler.Post(url, passwordChangeJsonData, WebRequestBodyType.JSON,
             ChangePasswordCallBack,
             ErrorChangePasswordCallBack,
             needHeaderAccessToken: true);
    }

    private void ChangePasswordCallBack(long code, string body) {
        OnChangePassword?.Invoke();
    }

    private void ErrorChangePasswordCallBack(long code, string body) {
        BadRequestChangePassword badRequestData;
        try {
            badRequestData = JsonUtility.FromJson<BadRequestChangePassword>(body);
        } catch (System.Exception) {
            badRequestData = new BadRequestChangePassword();
            badRequestData.code = code;
            badRequestData.errorMsg = body;
            OnErrorChangePassword?.Invoke(badRequestData);
            return;
        }
        OnErrorChangePassword?.Invoke(badRequestData);
    }
    #endregion

    private string GetRequestURL(string postfix) {
        return _webRequestHandler.ServerURLAuthAPI + postfix;
    }
}
