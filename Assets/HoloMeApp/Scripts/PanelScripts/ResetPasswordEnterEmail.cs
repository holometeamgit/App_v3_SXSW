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
        if(LocalDataVerification())
            SendEmail(emailInputField.text);
    }

    public string GetEmail() {
        return emailInputField.text;
    }

    public void ClearData() {
        emailInputField.text = "";
    }

    private void SendEmail(string email) {
        ResetPasswordEmailJsonData resetPasswordEmailJsonData = new ResetPasswordEmailJsonData(email);
        emailAccountManager.StartResetPassword(resetPasswordEmailJsonData);
    }

    private void StartResetPasswordCallBack() {
        switchToResetPasswordVerification.Switch();
    }

    private void ErrorStartResetPasswordBack(BadRequestStartResetPassword badRequestData) {
        emailInputField.ShowWarning("Server Error " + badRequestData.code.ToString());
        if (badRequestData == null)
            return;

        if (badRequestData.email.Count > 0)
            emailInputField.ShowWarning(badRequestData.email[0]);


    }

    private bool LocalDataVerification() {
        if (string.IsNullOrWhiteSpace(emailInputField.text))
            emailInputField.ShowWarning("This field is compulsory");

        return !string.IsNullOrWhiteSpace(emailInputField.text);
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
