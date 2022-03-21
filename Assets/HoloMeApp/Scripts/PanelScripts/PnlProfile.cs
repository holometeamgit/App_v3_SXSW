using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Beem.SSO;
using Zenject;

public class PnlProfile : MonoBehaviour {
    [SerializeField] GameObject InputDataArea;
    [SerializeField] InputFieldController usernameInputField;
    [SerializeField] int userNameLimit;

    [SerializeField] List<GameObject> backBtns;

    [SerializeField] ExternalLinkRedirector externalLinkRedirector;

    [SerializeField]
    private Toggle toggleEmailReceive;


    private AccountManager _accountManager;
    private UserWebManager _userWebManager;

    [Inject]
    public void Construct(AccountManager accountManager, UserWebManager userWebManager) {
        _accountManager = accountManager;
        _userWebManager = userWebManager;
    }

    public void ChooseUsername() {
        if (LocalDataVerification()) {
            _userWebManager.UpdateUserData(userName: usernameInputField.text);
            AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyProfileCreated, AnalyticParameters.ParamSignUpMethod, AnalyticsSignUpModeTracker.Instance.SignUpMethodUsed.ToString());
        }
    }

    private void Start() {
        usernameInputField.characterLimit = userNameLimit;
        _userWebManager.LoadUserInfo();
    }

    private void UserInfoLoadedCallBack() {
        usernameInputField.text = string.IsNullOrWhiteSpace(usernameInputField.text) ? _userWebManager.GetUsername() ?? "" : usernameInputField.text;
        if (_userWebManager.GetUsername() == null) {
            InputDataArea.SetActive(true);
        } else {
            SwitchToMainMenu();
        }
    }

    private void ErrorUserInfoLoadedCallBack() {
        ShowMsgForDeletedUser();
        ProfileToWelcome();
    }

    private void UpdateUserDataCallBack() {

        if (toggleEmailReceive.isOn) {
            AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyEmailOptIn, AnalyticParameters.ParamSignUpMethod, AnalyticsSignUpModeTracker.Instance.SignUpMethodUsed.ToString());
        }

        _userWebManager.LoadUserInfo();
        SwitchToMainMenu();
    }

    private void SwitchToMainMenu() {
        ProfileToMainMenu();
    }

    /// <summary>
    /// Switch profile to welcome
    /// </summary>
    public void ProfileToWelcome() {
        CreateUsernameConstructor.OnActivated?.Invoke(false);
        WelcomeConstructor.OnActivated?.Invoke(true);
        _accountManager.LogOut();
    }

    /// <summary>
    /// Switch profile to main menu
    /// </summary>
    public void ProfileToMainMenu() {
        CreateUsernameConstructor.OnActivated?.Invoke(false);
        MenuConstructor.OnActivated?.Invoke(true);
    }

    private void ErrorUpdateUserDataCallBack(BadRequestUserUploadJsonData badRequestData) {

        if (badRequestData == null ||
            (string.IsNullOrEmpty(badRequestData.username) &&
            badRequestData.first_name.Count == 0 &&
            badRequestData.last_name.Count == 0 &&
            string.IsNullOrEmpty(badRequestData.detail))) {
            usernameInputField.ShowWarning("Server Error " + badRequestData.code.ToString());
            return;
        }

        if (!string.IsNullOrEmpty(badRequestData.username)) {
            usernameInputField.ShowWarning(badRequestData.username);
        }

        if (!string.IsNullOrEmpty(badRequestData.detail))
            usernameInputField.ShowWarning(badRequestData.detail);
    }

    private void ClearInputFieldData() {
        usernameInputField.text = "";
    }

    private bool LocalDataVerification() {
        if (string.IsNullOrWhiteSpace(usernameInputField.text))
            usernameInputField.ShowWarning("This field is compulsory");
        else if (usernameInputField.text.Length > 20)
            usernameInputField.ShowWarning("Username must be 20 characters or less");

        return !string.IsNullOrWhiteSpace(usernameInputField.text) &&
            usernameInputField.text.Length <= 20;
    }

    private void ShowMsgForDeletedUser() {
        usernameInputField.MobileInputField.gameObject.SetActive(false);
        WarningConstructor.ActivateDoubleButton(null,
            string.Format("This account has been deleted, contact support to reinstate. "),
            "Support",
            "Cancel",
            () => { externalLinkRedirector.Redirect(); usernameInputField.MobileInputField.gameObject.SetActive(true); },
            () => { usernameInputField.MobileInputField.gameObject.SetActive(true); }
        );
    }

    private void OnEnable() {
        _userWebManager.OnUserInfoLoaded += UserInfoLoadedCallBack;
        _userWebManager.OnErrorUserInfoLoaded += ErrorUserInfoLoadedCallBack;
        _userWebManager.OnUserInfoUploaded += UpdateUserDataCallBack;
        _userWebManager.OnErrorUserUploaded += ErrorUpdateUserDataCallBack;

        InputDataArea.SetActive(false);
        _userWebManager.LoadUserInfo();

        toggleEmailReceive.isOn = false;
        toggleEmailReceive.enabled = false;
        toggleEmailReceive.enabled = true;
    }

    private void OnDisable() {
        _userWebManager.OnUserInfoLoaded -= UserInfoLoadedCallBack;
        _userWebManager.OnErrorUserInfoLoaded -= ErrorUserInfoLoadedCallBack;
        _userWebManager.OnUserInfoUploaded -= UpdateUserDataCallBack;
        _userWebManager.OnErrorUserUploaded -= ErrorUpdateUserDataCallBack;

        ClearInputFieldData();

        foreach (var backBtn in backBtns) {
            backBtn.SetActive(false);
        }
    }
}
