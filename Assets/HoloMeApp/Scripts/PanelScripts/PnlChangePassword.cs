using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class PnlChangePassword : MonoBehaviour
{
    [SerializeField] EmailAccountManager emailAccountManager;
    [SerializeField] InputFieldController newPasswordInputField;
    [SerializeField] InputFieldController newPasswordRepeatInputField;
    [SerializeField] Switcher switchToNectMenu;

    public void ChangePassword() {
        PasswordChangeJsonData passwordChangeJsonData = new PasswordChangeJsonData();
        passwordChangeJsonData.new_password1 = newPasswordInputField.text;
        passwordChangeJsonData.new_password2 = newPasswordRepeatInputField.text;

        emailAccountManager.ChangePassword(passwordChangeJsonData);
    }

    private void Start() {
        emailAccountManager = emailAccountManager ?? FindObjectOfType<EmailAccountManager>();
    }

    private void OnChangePasswordCallBack() {
        switchToNectMenu?.Switch();
    }

    private void OnErrorChangePasswordCallBack(BadRequestChangePassword badRequestChangePassword) {
        if (!string.IsNullOrWhiteSpace(badRequestChangePassword.detail))
            newPasswordInputField.ShowWarning(badRequestChangePassword.detail);

        if (badRequestChangePassword.new_password1.Count > 0)
            newPasswordInputField.ShowWarning(badRequestChangePassword.new_password1[0]);
        if (badRequestChangePassword.new_password2.Count > 0)
            newPasswordRepeatInputField.ShowWarning(badRequestChangePassword.new_password2[0]);
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