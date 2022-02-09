using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WindowManager.Extenject;
using Zenject;

public class PnlChangeUsername : MonoBehaviour {
    [SerializeField] InputFieldController usernameInputField;
    [SerializeField] int userNameLimit = 30;
    private UserWebManager _userWebManager;

    [Inject]
    public void Construct(UserWebManager userWebManager) {
        _userWebManager = userWebManager;
    }

    public void ChangeUsername() {
        if (LocalDataVerification())
            _userWebManager.UpdateUserData(userName: usernameInputField?.text ?? null);
    }

    private void Start() {
        usernameInputField.characterLimit = userNameLimit;
    }

    private void UserInfoLoadedCallBack() {
        usernameInputField.text = string.IsNullOrWhiteSpace(usernameInputField.text) ? _userWebManager.GetUsername() ?? "" : usernameInputField.text;
    }

    private void UpdateUserDataCallBack() {
        usernameInputField.MobileInputField.gameObject.SetActive(false);

        GeneralPopUpData.ButtonData closeButton = new GeneralPopUpData.ButtonData("Continue", () => { ChangeUserNameToSettings(); usernameInputField.MobileInputField.gameObject.SetActive(true); });
        GeneralPopUpData data = new GeneralPopUpData(null, "Username has been successfully updated", closeButton);

        WarningConstructor.OnShow?.Invoke(data);
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

    private bool LocalDataVerification() {
        if (string.IsNullOrWhiteSpace(usernameInputField.text))
            usernameInputField.ShowWarning("This field is compulsory");
        else if (usernameInputField.text.Length > 20)
            usernameInputField.ShowWarning("Username must be 20 characters or less");

        return !string.IsNullOrWhiteSpace(usernameInputField.text) &&
            usernameInputField.text.Length <= 20;
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
        ChangeUsernameConstructor.OnActivated?.Invoke(false);
        SettingsConstructor.OnActivated?.Invoke(true);
    }

    private void OnDisable() {
        _userWebManager.OnUserInfoLoaded -= UserInfoLoadedCallBack;
        _userWebManager.OnUserInfoUploaded -= UpdateUserDataCallBack;
        _userWebManager.OnErrorUserUploaded -= ErrorUpdateUserDataCallBack;
    }
}
