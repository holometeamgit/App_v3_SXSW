using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class PnlChangePassword : MonoBehaviour {
    [SerializeField] PnlGenericError pnlGenericError;
    [SerializeField] EmailAccountManager emailAccountManager;
    [SerializeField] InputFieldController newPasswordInputField;
    [SerializeField] InputFieldController newPasswordRepeatInputField;
    [SerializeField] Switcher switchToNextMenu;

    public void ChangePassword() {
        if (!LocalDataVerification())
            return;

        PasswordChangeJsonData passwordChangeJsonData = new PasswordChangeJsonData();
        passwordChangeJsonData.new_password1 = newPasswordInputField.text;
        passwordChangeJsonData.new_password2 = newPasswordRepeatInputField.text;

        emailAccountManager.ChangePassword(passwordChangeJsonData);
    }

    private void Start() {
        emailAccountManager = emailAccountManager ?? FindObjectOfType<EmailAccountManager>();
        newPasswordInputField.IsClearOnDisable = true;
        newPasswordRepeatInputField.IsClearOnDisable = true;
    }

    private void OnChangePasswordCallBack() {
        GenericConstructor.ActivateSingleButton(" ", "Password has been successfully updated", "Continue", () => switchToNextMenu.Switch());
    }

    private void OnErrorChangePasswordCallBack(BadRequestChangePassword badRequestChangePassword) {
        if (badRequestChangePassword == null ||
        (badRequestChangePassword.new_password1.Count == 0 &&
        badRequestChangePassword.new_password2.Count == 0 &&
        string.IsNullOrEmpty(badRequestChangePassword.detail))) {
            newPasswordInputField.ShowWarning("Server Error " + badRequestChangePassword.code.ToString());
            return;
        }

        if (!string.IsNullOrWhiteSpace(badRequestChangePassword.detail))
            newPasswordInputField.ShowWarning(badRequestChangePassword.detail);

        if (badRequestChangePassword.new_password1.Count > 0)
            newPasswordInputField.ShowWarning(badRequestChangePassword.new_password1[0]);
        if (badRequestChangePassword.new_password2.Count > 0) {
            newPasswordRepeatInputField.ShowWarning(badRequestChangePassword.new_password2[0]);
        }
    }

    private bool LocalDataVerification() {
        if (string.IsNullOrWhiteSpace(newPasswordInputField.text))
            newPasswordInputField.ShowWarning("This field is compulsory");
        if (string.IsNullOrWhiteSpace(newPasswordRepeatInputField.text))
            newPasswordRepeatInputField.ShowWarning("This field is compulsory");

        return !string.IsNullOrWhiteSpace(newPasswordInputField.text) &&
            !string.IsNullOrWhiteSpace(newPasswordRepeatInputField.text);
    }

    private void OnEnable() {
        emailAccountManager.OnChangePassword += OnChangePasswordCallBack;
        emailAccountManager.OnErrorChangePassword += OnErrorChangePasswordCallBack;
    }

    private void OnDisable() {
        emailAccountManager.OnChangePassword -= OnChangePasswordCallBack;
        emailAccountManager.OnErrorChangePassword -= OnErrorChangePasswordCallBack;
    }
}