using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PnlProfile : MonoBehaviour
{
    [SerializeField] UserWebManager userWebManager;
    [SerializeField] GameObject InputDataArea;
    [SerializeField] InputFieldController userNameInputField;
    [SerializeField] InputFieldController fullNameInputField;
    [SerializeField] Switcher switchToMainMenu;

    [SerializeField] List<GameObject> backBtns;

    public void ChooseUsername() {
        userWebManager.UpdateUserData(userName: userNameInputField?.text ?? null, first_name: fullNameInputField?.text ?? null);
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
        if (userNameInputField != null)
            userNameInputField.text = userNameInputField.text == "" ? userWebManager.GetUserName() ?? "" : userNameInputField.text;

        if (fullNameInputField != null)
            fullNameInputField.text = fullNameInputField.text == "" ? userWebManager.GetFullName() ?? "" : fullNameInputField.text;

        if ((userWebManager.GetUserName() == null && userNameInputField != null) ||
            userWebManager.GetFullName() == null && fullNameInputField != null) {
            InputDataArea.SetActive(true);
        } else {
            switchToMainMenu.Switch();
        }
    }

    private void UpdateUserDataCallBack() {
        userWebManager.LoadUserInfo();
        switchToMainMenu.Switch();
    }

    private void ErrorUpdateUserDataCallBack(BadRequestUserUploadJsonData badRequestData) {
        if (!this.isActiveAndEnabled)
            return;

        if (badRequestData.username.Count > 0)
            userNameInputField.ShowWarning(badRequestData.username[0]);

        if (badRequestData.first_name.Count > 0)
            fullNameInputField.ShowWarning(badRequestData.first_name[0]);

        if (!string.IsNullOrEmpty(badRequestData.detail))
            userNameInputField.ShowWarning(badRequestData.detail);
    }

    private void OnEnable() {
        InputDataArea.SetActive(false);
        userWebManager.LoadUserInfo();
    }

    private void OnDisable() {
        foreach(var backBtn in backBtns) {
            backBtn.SetActive(false);
        }
    }
}
