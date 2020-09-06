﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlLogInEmail : MonoBehaviour
{
    [SerializeField] EmailAccountManager emailAccountManager;
    [SerializeField] InputFieldController inputFieldEmail;
    [SerializeField] InputFieldController inputFieldPassword;
    [SerializeField] Switcher switcherToProfile;

    public void LogIn() {
        EmailLogInJsonData emailLogInJsonData = new EmailLogInJsonData();

        emailLogInJsonData.username = inputFieldEmail.text;
        emailLogInJsonData.password = inputFieldPassword.text;

        emailAccountManager.LogIn(emailLogInJsonData);
    }

    private void Start() {
        emailAccountManager.OnLogIn += LogInCallBack;
        emailAccountManager.OnErrorLogIn += ErrorLogInCallBack;
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

        if (badRequestData.password.Count > 0)
            inputFieldPassword.ShowWarning(badRequestData.password[0]);

        if(!string.IsNullOrEmpty(badRequestData.detail))
            inputFieldEmail.ShowWarning(badRequestData.detail);
    }
}
