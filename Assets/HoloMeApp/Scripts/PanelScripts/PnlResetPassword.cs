using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PnlResetPassword : MonoBehaviour {

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
        if (!LocalDataVerification())
            return;

        ResetPasswordJsonData resetPasswordJsonData =
            new ResetPasswordJsonData(passwordInputField.text, confirmPasswordInputField.text, uid, token);
        emailAccountManager.ResetPassword(resetPasswordJsonData);
    }

    public void AddVerificationData(string uid, string token) {
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
        resetPasswordEnterEmail.ClearData();
        WarningConstructor.ActivateSingleButton(" ", "Password has been successfully updated", "Continue", () => switcherToResetPassword.Switch());
    }

    private void ErrorResetPasswordCallBack(BadRequestResetPassword badRequestResetPassword) {
        if (badRequestResetPassword == null) {
            passwordInputField.ShowWarning("Server Error " + badRequestResetPassword.code.ToString());
            return;
        }

        bool hasMsg = false;
        if (badRequestResetPassword.uid.Count > 0) {
            passwordInputField.ShowWarning("Outdated confirmation code in your email");//badRequestResetPassword.uid[0]); //Outdated confirmation code in your email
            EnableVerificationInfoResend();
            hasMsg = true;
        }
        if (badRequestResetPassword.token.Count > 0) {
            passwordInputField.ShowWarning("Outdated confirmation code in your email");
            EnableVerificationInfoResend();
            hasMsg = true;
        }
        if (badRequestResetPassword.new_password1.Count > 0) {
            passwordInputField.ShowWarning(badRequestResetPassword.new_password1[0]);
            hasMsg = true;
        }
        if (badRequestResetPassword.new_password2.Count > 0) {
            hasMsg = true;
            confirmPasswordInputField.ShowWarning(badRequestResetPassword.new_password2[0]);
        }

        if (!hasMsg)
            passwordInputField.ShowWarning("Server Error " + badRequestResetPassword.code.ToString());
    }

    private bool LocalDataVerification() {
        if (string.IsNullOrWhiteSpace(passwordInputField.text))
            passwordInputField.ShowWarning("This field is compulsory");
        if (string.IsNullOrWhiteSpace(confirmPasswordInputField.text))
            confirmPasswordInputField.ShowWarning("This field is compulsory");

        return !string.IsNullOrWhiteSpace(passwordInputField.text) &&
            !string.IsNullOrWhiteSpace(confirmPasswordInputField.text);
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
