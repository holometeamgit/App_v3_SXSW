using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlLogInEmail : MonoBehaviour {
    [SerializeField] EmailAccountManager emailAccountManager;
    [SerializeField] InputFieldController inputFieldEmail;
    [SerializeField] InputFieldController inputFieldPassword;
    [SerializeField] Switcher switcherToProfile;

    public void LogIn() {
        EmailLogInJsonData emailLogInJsonData = new EmailLogInJsonData();

        emailLogInJsonData.username = inputFieldEmail.text;
        emailLogInJsonData.password = inputFieldPassword.text;

        if (string.IsNullOrWhiteSpace(inputFieldEmail.text) || string.IsNullOrWhiteSpace(inputFieldPassword.text))
            InitWarningMsg();
        else
            emailAccountManager.LogIn(emailLogInJsonData);
    }

    private void LogInCallBack() {
        if (!this.isActiveAndEnabled)
            return;
        switcherToProfile.Switch();
    }

    private void ErrorLogInCallBack(BadRequestLogInEmailJsonData badRequestData) {
        if (!this.isActiveAndEnabled)
            return;

        if (badRequestData.username.Count > 0)
            inputFieldEmail.ShowWarning(badRequestData.username[0]);

        if (badRequestData.non_field_errors.Count > 0) {
            if (badRequestData.non_field_errors[0] != "E-mail is not verified.")
                inputFieldEmail.ShowWarning(badRequestData.non_field_errors[0]);
            else {
                inputFieldEmail.ShowWarning("E-mail is not verified. We had sent a verification email");
                //ResendVerifyJsonData resendVerifyJsonData = new ResendVerifyJsonData(inputFieldEmail.text);
                //emailAccountManager.ResendVerification(resendVerifyJsonData);
            }
        }

        if (badRequestData.password.Count > 0)
            inputFieldPassword.ShowWarning(badRequestData.password[0]);

        if (!string.IsNullOrEmpty(badRequestData.detail))
            //inputFieldEmail.ShowWarning(badRequestData.detail);
            inputFieldPassword.ShowWarning("Incorrect password");
    }

    private void ResendVerificationCallBack() {
        if (!this.isActiveAndEnabled)
            return;

        inputFieldEmail.ShowWarning("E-mail is not verified. We have sent a verification email");
    }

    private void ErrorResendVerificationCallBack(BadRequestResendVerificationJsonData badRequestData) {
        if (!this.isActiveAndEnabled)
            return;

        if (badRequestData.email.Count > 0)
            inputFieldEmail.ShowWarning(badRequestData.email[0]);
    }

    private void InitWarningMsg() {
        BadRequestLogInEmailJsonData badRequestLogInEmailJsonData = new BadRequestLogInEmailJsonData();
        Debug.Log(inputFieldEmail.text);
        if (string.IsNullOrWhiteSpace(inputFieldEmail.text))
            badRequestLogInEmailJsonData.username.Insert(0,"Required");

        if (string.IsNullOrWhiteSpace(inputFieldPassword.text))
            badRequestLogInEmailJsonData.password.Insert(0,"Required");

        ErrorLogInCallBack(badRequestLogInEmailJsonData);
    }

    private void OnEnable() {
        emailAccountManager.OnLogIn += LogInCallBack;
        emailAccountManager.OnErrorLogIn += ErrorLogInCallBack;
        emailAccountManager.OnResendVerification += ResendVerificationCallBack;
        emailAccountManager.OnErrorResendVerification += ErrorResendVerificationCallBack;
    }

    private void OnDisable() {
        emailAccountManager.OnLogIn -= LogInCallBack;
        emailAccountManager.OnErrorLogIn -= ErrorLogInCallBack;
        emailAccountManager.OnResendVerification -= ResendVerificationCallBack;
        emailAccountManager.OnErrorResendVerification -= ErrorResendVerificationCallBack;
    }
}
