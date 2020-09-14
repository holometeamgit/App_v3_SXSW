using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PnlResetPassword : MonoBehaviour
{
    [SerializeField]
    EmailAccountManager emailAccountManager;
    [SerializeField]
    ResetPasswordEnterEmail resetPasswordEnterEmail;
    [SerializeField]
    DeepLinkHandler deepLinkHandler;
    [SerializeField]
    InputFieldController passwordInputField;
    [SerializeField]
    InputFieldController confirmPasswordInputField;

    [SerializeField]
    Switcher switcherToResetPassword;

    [SerializeField]
    GameObject ResendBtn;
    [SerializeField]
    GameObject rectVerificationInfo;
    [SerializeField]
    GameObject rectInputFieldChangePassword;
    [SerializeField]
    TMP_Text txtEmail;

    private string uid;
    private string token;

    public void ResendVerification() {
        resetPasswordEnterEmail.SendEmail();
        EnableVerificationInfo();
    }

    public void ResetPassword() {
        ResetPasswordJsonData resetPasswordJsonData =
            new ResetPasswordJsonData(passwordInputField.text, confirmPasswordInputField.text, uid, token);
        emailAccountManager.ResetPassword(resetPasswordJsonData);
    }

    private void AddVerificationData(string uid, string token) {
        Debug.Log("Verify " + uid + " token " + token);
        this.uid = uid;
        this.token = token;
        StartInputPasswordData();
    }

    private void StartInputPasswordData() {
        rectVerificationInfo.SetActive(false);
        rectInputFieldChangePassword.SetActive(true);

        ResendBtn?.gameObject.SetActive(false);
    }

    private void EnableVerificationInfo() {
        rectVerificationInfo.SetActive(true);
        rectInputFieldChangePassword.SetActive(false);

        ResendBtn?.gameObject.SetActive(false);
    }

    private void EnableVerificationInfoResend() {
        ResendBtn?.gameObject.SetActive(true);
    }

    private void ResetPasswordCallBack() {
        switcherToResetPassword.Switch();
    }

    private void ErrorResetPasswordCallBack(BadRequestResetPassword badRequestResetPassword) {
        if (badRequestResetPassword.new_password1.Count > 0)
            passwordInputField.ShowWarning(badRequestResetPassword.new_password1[0]);
        if (badRequestResetPassword.uid.Count > 0) {
            //    passwordInputField.ShowWarning(badRequestResetPassword.uid[0]);
            EnableVerificationInfoResend();
        }
        if (badRequestResetPassword.token.Count > 0) {
            //passwordInputField.ShowWarning(badRequestResetPassword.token[0]);
            EnableVerificationInfoResend();
        }
        if (badRequestResetPassword.new_password2.Count > 0)
            passwordInputField.ShowWarning(badRequestResetPassword.new_password2[0]);

        passwordInputField.ShowWarning("Outdated confirmation code in your email");
    }

    private void OnEnable() {
        emailAccountManager.OnResetPassword += ResetPasswordCallBack;
        emailAccountManager.OnErrorResetPassword += ErrorResetPasswordCallBack;

        if (txtEmail != null)
            txtEmail.text = resetPasswordEnterEmail.GetEmail();

        EnableVerificationInfo();

        deepLinkHandler.PasswordResetConfirmDeepLinkActivated += AddVerificationData;
    }

    private void OnDisable() {
        emailAccountManager.OnResetPassword -= ResetPasswordCallBack;
        emailAccountManager.OnErrorResetPassword -= ErrorResetPasswordCallBack;

        deepLinkHandler.PasswordResetConfirmDeepLinkActivated -= AddVerificationData;
    }
}
