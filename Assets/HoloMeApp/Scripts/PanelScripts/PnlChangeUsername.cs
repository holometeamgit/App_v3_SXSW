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
        userWebManager.OnUserInfoLoaded += UserInfoLoadedCallBack;
        userWebManager.OnUserInfoUploaded += UpdateUserDataCallBack;
        userWebManager.OnErrorUserUploaded += ErrorUpdateUserDataCallBack;
        userWebManager.LoadUserInfo();
    }

    private void UserInfoLoadedCallBack() {
        if (!this.isActiveAndEnabled)
            return;

        usernameInputField.text = usernameInputField.text == "" ? userWebManager.GetUsername() ?? "" : usernameInputField.text;
    }

    private void UpdateUserDataCallBack() {
        if (!this.isActiveAndEnabled)
            return;
        switchToSetting.Switch();
    }

    private void ErrorUpdateUserDataCallBack(BadRequestUserUploadJsonData badRequestData) {
        if (!this.isActiveAndEnabled)
            return;

        if (badRequestData.username.Count > 0)
            usernameInputField.ShowWarning(badRequestData.username[0]);

        if (!string.IsNullOrEmpty(badRequestData.detail))
            usernameInputField.ShowWarning(badRequestData.detail);
    }
}
