using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlCheckPassword : MonoBehaviour
{
    [SerializeField] UserWebManager userWebManager;
    [SerializeField] EmailAccountManager emailAccountManager;
    [SerializeField] InputFieldController inputFieldPassword;
    [SerializeField] Switcher SwitchToNextMenu;

    public void CheckPassword() {
        if (string.IsNullOrWhiteSpace(inputFieldPassword.text))
            InitWarningMsg();
        else
        userWebManager.LoadUserInfo();
    }

    private void CheckAccess() {
        EmailLogInJsonData emailLogInJsonData = new EmailLogInJsonData();

        emailLogInJsonData.username = userWebManager.GetUsername();
        emailLogInJsonData.password = inputFieldPassword.text;

        emailAccountManager.LogIn(emailLogInJsonData);
    }

    private void LogInCallBack() {
        SwitchToNextMenu.Switch();
    }

    private void ErrorLogInCallBack(BadRequestLogInEmailJsonData badRequestData) {
        if (badRequestData.password.Count > 0)
            inputFieldPassword.ShowWarning(badRequestData.password[0]);

        else if (!string.IsNullOrEmpty(badRequestData.detail))
            inputFieldPassword.ShowWarning("Incorrect password");

        else
            inputFieldPassword.ShowWarning("Incorrect password");
    }

    private void InitWarningMsg() {
        BadRequestLogInEmailJsonData badRequestLogInEmailJsonData = new BadRequestLogInEmailJsonData();

        if (string.IsNullOrWhiteSpace(inputFieldPassword.text))
            badRequestLogInEmailJsonData.password.Insert(0, "Required");

        ErrorLogInCallBack(badRequestLogInEmailJsonData);
    }

    private void OnEnable() {
        emailAccountManager.OnLogIn += LogInCallBack;
        emailAccountManager.OnErrorLogIn += ErrorLogInCallBack;

        userWebManager.OnUserInfoLoaded += CheckAccess;
        inputFieldPassword.text = "";
    }

    private void OnDisable() {
        emailAccountManager.OnLogIn -= LogInCallBack;
        emailAccountManager.OnErrorLogIn -= ErrorLogInCallBack;

        userWebManager.OnUserInfoLoaded -= CheckAccess;
        inputFieldPassword.text = "";
    }

}
