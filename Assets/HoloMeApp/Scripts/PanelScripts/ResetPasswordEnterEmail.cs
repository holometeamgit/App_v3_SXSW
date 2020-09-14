using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPasswordEnterEmail : MonoBehaviour
{
    [SerializeField]
    EmailAccountManager emailAccountManager;
    [SerializeField]
    InputFieldController emailInputField;
    [SerializeField]
    Switcher switchToResetPasswordVerification;

    public void SendEmail() {
        if (emailInputField == null)
            return;
        SendEmail(emailInputField.text);
    }

    public string GetEmail() {
        return emailInputField.text;
    }

    private void SendEmail(string email) {
        ResetPasswordEmailJsonData resetPasswordEmailJsonData = new ResetPasswordEmailJsonData(email);
        emailAccountManager.StartResetPassword(resetPasswordEmailJsonData);
    }

    private void StartResetPasswordCallBack() {
        switchToResetPasswordVerification.Switch();
    }

    private void ErrorStartResetPasswordBack(BadRequestStartResetPassword badRequestData) {
        if (badRequestData.email.Count > 0)
            emailInputField.ShowWarning(badRequestData.email[0]);
    }

    private void OnEnable() {
        emailAccountManager.OnStartResetPassword += StartResetPasswordCallBack;
        emailAccountManager.OnErrorStartResetPassword += ErrorStartResetPasswordBack;
    }

    private void OnDisable() {
        emailAccountManager.OnStartResetPassword -= StartResetPasswordCallBack;
        emailAccountManager.OnErrorStartResetPassword -= ErrorStartResetPasswordBack;
    }

}
