using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using Zenject;

public class PnlCheckPassword : MonoBehaviour {
    [SerializeField] UserWebManager userWebManager;
    [SerializeField] EmailAccountManager emailAccountManager;
    [SerializeField] InputFieldController inputFieldPassword;
    [SerializeField] Switcher SwitchToNextMenu;

    private AccountManager _accountManager;

    [Inject]
    public void Construct(AccountManager accountManager) {
        _accountManager = accountManager;
    }

    public void CheckPassword() {
        if (LocalDataVerification())
            userWebManager.LoadUserInfo();
    }

    private void Start() {
        inputFieldPassword.IsClearOnDisable = true;
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

    private bool LocalDataVerification() {
        if (string.IsNullOrWhiteSpace(inputFieldPassword.text))
            inputFieldPassword.ShowWarning("This field is compulsory");

        return !string.IsNullOrWhiteSpace(inputFieldPassword.text);
    }

    private void OnEnable() {
        emailAccountManager.OnLogIn += LogInCallBack;
        emailAccountManager.OnErrorLogIn += ErrorLogInCallBack;

        userWebManager.OnUserInfoLoaded += CheckAccess;

        if (_accountManager.GetLogInType() != LogInType.Email)
            SwitchToNextMenu.Switch();
    }

    private void OnDisable() {
        emailAccountManager.OnLogIn -= LogInCallBack;
        emailAccountManager.OnErrorLogIn -= ErrorLogInCallBack;

        userWebManager.OnUserInfoLoaded -= CheckAccess;
    }

}
