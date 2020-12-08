using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlChangeUsername : MonoBehaviour
{
    [SerializeField] PnlGenericError pnlGenericError;
    [SerializeField] UserWebManager userWebManager;
    [SerializeField] InputFieldController usernameInputField;
    [SerializeField] Switcher switchToSetting;

    public void ChangeUsername() {
        if(LocalDataVerification())
            userWebManager.UpdateUserData(userName: usernameInputField?.text ?? null);
    }

    private void Start() {
        userWebManager.LoadUserInfo();
    }

    private void UserInfoLoadedCallBack() {
        usernameInputField.text = usernameInputField.text == "" ? userWebManager.GetUsername() ?? "" : usernameInputField.text;
    }

    private void UpdateUserDataCallBack() {
        pnlGenericError.ActivateSingleButton(" ", "Username has been successfully updated", "Continue", () => switchToSetting.Switch());
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
        userWebManager.OnUserInfoLoaded += UserInfoLoadedCallBack;
        userWebManager.OnUserInfoUploaded += UpdateUserDataCallBack;
        userWebManager.OnErrorUserUploaded += ErrorUpdateUserDataCallBack;
    }

    private void OnDisable() {
        userWebManager.OnUserInfoLoaded -= UserInfoLoadedCallBack;
        userWebManager.OnUserInfoUploaded -= UpdateUserDataCallBack;
        userWebManager.OnErrorUserUploaded -= ErrorUpdateUserDataCallBack;
    }
}
