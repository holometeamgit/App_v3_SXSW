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
        userWebManager.OnUserInfoLoaded += UserInfoLoadedCallBack;
        userWebManager.OnUserInfoUploaded += UpdateUserDataCallBack;
        userWebManager.OnErrorUserUploaded += ErrorUpdateUserDataCallBack;
        userWebManager.LoadUserInfo();
    }

    private void UserInfoLoadedCallBack() {
        if (!this.isActiveAndEnabled)
            return;

        bioInputField.text = bioInputField.text == "" ? userWebManager.GetBio() ?? "" : bioInputField.text;
    }

    private void UpdateUserDataCallBack() {
        if (!this.isActiveAndEnabled)
            return;
        switchToSetting.Switch();
    }

    private void ErrorUpdateUserDataCallBack(BadRequestUserUploadJsonData badRequestData) {
        if (!this.isActiveAndEnabled)
            return;
        //TODO: Add after back will add this
    }
}
