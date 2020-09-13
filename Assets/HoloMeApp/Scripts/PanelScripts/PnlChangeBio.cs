using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlChangeBio : MonoBehaviour
{
    [SerializeField] UserWebManager userWebManager;
    [SerializeField] InputFieldController bioInputField;
    [SerializeField] Switcher switchToSetting;

    public void ChangeBio() {
        userWebManager.UpdateUserData(bio: bioInputField?.text ?? null);
    }

    private void Start() {
        userWebManager.LoadUserInfo();
    }

    private void UserInfoLoadedCallBack() {
        if (!this.isActiveAndEnabled)
            return;

        bioInputField.text = bioInputField.text == "" ? userWebManager.GetBio() ?? "" : bioInputField.text;
    }

    private void UpdateUserDataCallBack() {
        switchToSetting.Switch();
    }

    private void ErrorUpdateUserDataCallBack(BadRequestUserUploadJsonData badRequestData) {
        //TODO: Add after back will add this
        bioInputField.ShowWarning("Incorrect symbols in bio");
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
