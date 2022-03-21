using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using Zenject;

public class PnlCheckPassword : MonoBehaviour {
    [SerializeField] EmailAccountManager emailAccountManager;
    [SerializeField] InputFieldController inputFieldPassword;
    [SerializeField] Switcher SwitchToNextMenu;

    private AccountManager _accountManager;
    private UserWebManager _userWebManager;

    [Inject]
    public void Construct(AccountManager accountManager, UserWebManager userWebManager) {
        _accountManager = accountManager;
        _userWebManager = userWebManager;
    }

    public void CheckPassword() {
        if (LocalDataVerification())
            _userWebManager.LoadUserInfo();
    }

    private void Start() {
        inputFieldPassword.IsClearOnDisable = true;
    }

    private void CheckAccess() {
        EmailLogInJsonData emailLogInJsonData = new EmailLogInJsonData();

        emailLogInJsonData.username = _userWebManager.GetUsername();
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

        _userWebManager.OnUserInfoLoaded += CheckAccess;

        if (_accountManager.GetLogInType() != LogInType.Email)
            SwitchToNextMenu.Switch();
    }

    private void OnDisable() {
        emailAccountManager.OnLogIn -= LogInCallBack;
        emailAccountManager.OnErrorLogIn -= ErrorLogInCallBack;

        _userWebManager.OnUserInfoLoaded -= CheckAccess;
    }

}
