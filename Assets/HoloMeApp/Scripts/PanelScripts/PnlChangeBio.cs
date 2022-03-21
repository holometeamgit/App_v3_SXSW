using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PnlChangeBio : MonoBehaviour {
    [SerializeField] InputFieldController bioInputField;
    [SerializeField] Switcher switchToSetting;

    private UserWebManager _userWebManager;

    [Inject]
    public void Construct(UserWebManager userWebManager) {
        _userWebManager = userWebManager;
    }

    public void ChangeBio() {
        _userWebManager.UpdateUserData(bio: bioInputField?.text ?? null);
    }

    private void Start() {
        _userWebManager.LoadUserInfo();
    }

    private void UserInfoLoadedCallBack() {
        if (!this.isActiveAndEnabled)
            return;

        bioInputField.text = bioInputField.text == "" ? _userWebManager.GetBio() ?? "" : bioInputField.text;
    }

    private void UpdateUserDataCallBack() {
        switchToSetting.Switch();
    }

    private void ErrorUpdateUserDataCallBack(BadRequestUserUploadJsonData badRequestData) {
        //TODO: Add after back will add this
        bioInputField.ShowWarning("Incorrect symbols in bio");
    }

    private void OnEnable() {
        _userWebManager.OnUserInfoLoaded += UserInfoLoadedCallBack;
        _userWebManager.OnUserInfoUploaded += UpdateUserDataCallBack;
        _userWebManager.OnErrorUserUploaded += ErrorUpdateUserDataCallBack;
    }

    private void OnDisable() {
        _userWebManager.OnUserInfoLoaded -= UserInfoLoadedCallBack;
        _userWebManager.OnUserInfoUploaded -= UpdateUserDataCallBack;
        _userWebManager.OnErrorUserUploaded -= ErrorUpdateUserDataCallBack;
    }
}
