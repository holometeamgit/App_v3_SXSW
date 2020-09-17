using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlChangeUsername : MonoBehaviour
{
    [SerializeField] UserWebManager userWebManager;
    [SerializeField] InputFieldController usernameInputField;
    [SerializeField] Switcher switchToSetting;

    public void ChangeUsername() {
        userWebManager.UpdateUserData(userName: usernameInputField?.text ?? null);
    }

    private void Start() {
        userWebManager.LoadUserInfo();
    }

    private void UserInfoLoadedCallBack() {
        usernameInputField.text = usernameInputField.text == "" ? userWebManager.GetUsername() ?? "" : usernameInputField.text;
    }

    private void UpdateUserDataCallBack() {
        switchToSetting.Switch();
    }

    private void ErrorUpdateUserDataCallBack(BadRequestUserUploadJsonData badRequestData) {
        if (badRequestData.username.Count > 0)
            usernameInputField.ShowWarning(badRequestData.username[0]);

        if (!string.IsNullOrEmpty(badRequestData.detail))
            usernameInputField.ShowWarning(badRequestData.detail);
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
