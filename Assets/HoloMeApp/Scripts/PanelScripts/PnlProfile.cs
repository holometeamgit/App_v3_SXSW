using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Beem.SSO;
using Zenject;
using System;

public class PnlProfile : MonoBehaviour {
    [SerializeField] GameObject InputDataArea;
    [SerializeField]
    private InputFieldController _usernameInputField;
    [SerializeField]
    private InputFieldController _phoneInputField;
    [SerializeField]
    private GameObject _smsBtn;
    [SerializeField]
    private InputFieldController _verificationCodeInputField;

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

    private string GetUserName {
        get {
            string username = RegexAlphaNumeric.RegexResult(_usernameInputField?.text);
            username = username.ToLower();
            return username;
        }
    }

    /// <summary>
    /// Choose Username
    /// </summary>
    public void ChooseUsername() {
        if (LocalDataVerification(GetUserName)) {
            _userWebManager.UpdateUserData(userName: GetUserName);
            AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyProfileCreated, AnalyticParameters.ParamSignUpMethod, AnalyticsSignUpModeTracker.Instance.SignUpMethodUsed.ToString());
        }
    }

    /// <summary>
    /// Send Sms
    /// </summary>
    public void SendSms() {
        if (LocalVerificationCode(_phoneInputField)) {
            CallBacks.onVerifiedPhone?.Invoke(_phoneInputField.text);
            _smsBtn.SetActive(false);
            _verificationCodeInputField.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Continue
    /// </summary>
    public void Continue() {
#if UNITY_EDITOR
        ChooseUsername();
#else
    SendVerificationCode();
#endif
    }

    /// <summary>
    /// Send VerificationCode
    /// </summary>
    public void SendVerificationCode() {
        if (LocalVerificationCode(_verificationCodeInputField)) {
            CallBacks.onSignInPhone?.Invoke(_verificationCodeInputField.text);
        }
    }

    private bool LocalVerificationCode(InputFieldController inputField) {
        if (string.IsNullOrWhiteSpace(inputField.text)) {
            inputField.ShowWarning("This field is compulsory");
        }

        return !string.IsNullOrWhiteSpace(inputField.text);
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
        OnboardingConstructor.OnActivated?.Invoke(true);
    }


    private void Start() {
        _usernameInputField.characterLimit = userNameLimit;
        _userWebManager.LoadUserInfo();
    }

    private void UserInfoLoadedCallBack() {
        _usernameInputField.text = string.IsNullOrWhiteSpace(GetUserName) ? _userWebManager.GetUsername() ?? "" : _usernameInputField.text;
        //if (_userWebManager.GetUsername() == null) {
        InputDataArea.SetActive(true);
        //} else {
        //    SwitchToMainMenu();
        //}
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



    private void ErrorUpdateUserDataCallBack(BadRequestUserUploadJsonData badRequestData) {

        if (badRequestData == null ||
            (string.IsNullOrEmpty(badRequestData.username) &&
            badRequestData.first_name.Count == 0 &&
            badRequestData.last_name.Count == 0 &&
            string.IsNullOrEmpty(badRequestData.detail))) {
            _usernameInputField.ShowWarning("Server Error " + badRequestData.code.ToString());
            return;
        }

        if (!string.IsNullOrEmpty(badRequestData.username)) {
            _usernameInputField.ShowWarning(badRequestData.username);
        }

        if (!string.IsNullOrEmpty(badRequestData.detail))
            _usernameInputField.ShowWarning(badRequestData.detail);
    }

    private void ClearInputFieldData() {
        _usernameInputField.text = "";
    }

    private bool LocalDataVerification(string text) {
        if (string.IsNullOrWhiteSpace(text))
            _usernameInputField.ShowWarning("This field is compulsory");
        else if (text.Length > 20)
            _usernameInputField.ShowWarning("Username must be 20 characters or less");

        return !string.IsNullOrWhiteSpace(text) &&
            _usernameInputField.text.Length <= 20;
    }

    private void ShowMsgForDeletedUser() {
        _usernameInputField.MobileInputField.gameObject.SetActive(false);
        WarningConstructor.ActivateDoubleButton(null,
            string.Format("This account has been deleted, contact support to reinstate. "),
            "Support",
            "Cancel",
            () => { externalLinkRedirector.Redirect(); _usernameInputField.MobileInputField.gameObject.SetActive(true); },
            () => { _usernameInputField.MobileInputField.gameObject.SetActive(true); }
        );
    }

    private void OnEnable() {
        _userWebManager.OnUserInfoLoaded += UserInfoLoadedCallBack;
        _userWebManager.OnErrorUserInfoLoaded += ErrorUserInfoLoadedCallBack;
        _userWebManager.OnUserInfoUploaded += UpdateUserDataCallBack;
        _userWebManager.OnErrorUserUploaded += ErrorUpdateUserDataCallBack;

        CallBacks.onFail += OnFailed;
        CallBacks.onFirebaseSignInSuccess += onSuccess;

        InputDataArea.SetActive(false);
        _userWebManager.LoadUserInfo();

        _smsBtn.SetActive(true);
        _verificationCodeInputField.gameObject.SetActive(false);
        toggleEmailReceive.isOn = false;
        toggleEmailReceive.enabled = false;
        toggleEmailReceive.enabled = true;
    }

    private void onSuccess(LogInType obj) {
        ChooseUsername();
    }

    private void OnFailed(string obj) {
        HelperFunctions.DevLogError(obj);
        if (obj.Contains("InvalidCode")) {
            _phoneInputField.ShowWarning("Verification Code is wrong");
        }
    }

    private void OnDisable() {
        _userWebManager.OnUserInfoLoaded -= UserInfoLoadedCallBack;
        _userWebManager.OnErrorUserInfoLoaded -= ErrorUserInfoLoadedCallBack;
        _userWebManager.OnUserInfoUploaded -= UpdateUserDataCallBack;
        _userWebManager.OnErrorUserUploaded -= ErrorUpdateUserDataCallBack;

        CallBacks.onFail -= OnFailed;
        CallBacks.onFirebaseSignInSuccess -= onSuccess;

        ClearInputFieldData();

        foreach (var backBtn in backBtns) {
            backBtn.SetActive(false);
        }
    }
}
