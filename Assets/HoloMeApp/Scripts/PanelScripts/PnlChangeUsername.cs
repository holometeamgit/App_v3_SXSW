using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PnlChangeUsername : MonoBehaviour {
    [SerializeField] InputFieldController usernameInputField;
    [SerializeField] int userNameLimit = 30;
    [Space]
    [SerializeField]
    private UserWebManager _userWebManager;

    private string GetUserName {
        get {
            return usernameInputField?.text;
        }
    }

    public void ChangeUsername() {
        if (LocalDataVerification(GetUserName))
            _userWebManager.UpdateUserData(userName: GetUserName);
    }

    private void Start() {
        usernameInputField.characterLimit = userNameLimit;
    }

    private void UserInfoLoadedCallBack() {
        usernameInputField.text = string.IsNullOrWhiteSpace(GetUserName) ? _userWebManager.GetUsername() ?? "" : usernameInputField.text;
    }

    private void UpdateUserDataCallBack() {
        usernameInputField.MobileInputField.gameObject.SetActive(false);
        WarningConstructor.ActivateSingleButton(" ", "Username has been successfully updated", "Continue", () => { ChangeUserNameToSettings(); usernameInputField.MobileInputField.gameObject.SetActive(true); });
    }

    private void ErrorUpdateUserDataCallBack(BadRequestUserUploadJsonData badRequestData) {
        if (!string.IsNullOrEmpty(badRequestData.username)) {
            /* if (badRequestData.username.Contains("is exist"))
                 usernameInputField.ShowWarning("Username already exists, please choose another");
             else*/
            usernameInputField.ShowWarning(badRequestData.username);
        }

        if (!string.IsNullOrEmpty(badRequestData.detail))
            usernameInputField.ShowWarning(badRequestData.detail);
    }

    private bool LocalDataVerification(string text) {
        if (string.IsNullOrWhiteSpace(text))
            usernameInputField.ShowWarning("This field is compulsory");
        else if (text.Length > 20)
            usernameInputField.ShowWarning("Username must be 20 characters or less");

        return !string.IsNullOrWhiteSpace(text) && text.Length <= 20;
    }

    private void OnEnable() {
        _userWebManager.OnUserInfoLoaded += UserInfoLoadedCallBack;
        _userWebManager.OnUserInfoUploaded += UpdateUserDataCallBack;
        _userWebManager.OnErrorUserUploaded += ErrorUpdateUserDataCallBack;
        usernameInputField.text = _userWebManager.GetUsername();
        _userWebManager.LoadUserInfo();
    }

    /// <summary>
    /// Back to settings
    /// </summary>
    public void ChangeUserNameToSettings() {
        SettingsConstructor.OnActivated?.Invoke(true);
        ChangeUsernameConstructor.OnActivated?.Invoke(false);
    }

    private void OnDisable() {
        _userWebManager.OnUserInfoLoaded -= UserInfoLoadedCallBack;
        _userWebManager.OnUserInfoUploaded -= UpdateUserDataCallBack;
        _userWebManager.OnErrorUserUploaded -= ErrorUpdateUserDataCallBack;
    }
}
