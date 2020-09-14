using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PnlProfile : MonoBehaviour
{
    [SerializeField] UserWebManager userWebManager;
    [SerializeField] GameObject InputDataArea;
    [SerializeField] InputFieldController usernameInputField;
    [SerializeField] InputFieldController firstnameInputField;
    [SerializeField] InputFieldController surnameInputField;
    [SerializeField] Switcher switchToMainMenu;

    [SerializeField] List<GameObject> backBtns;

    public void ChooseUsername() {
        userWebManager.UpdateUserData(userName: usernameInputField?.text ?? null,
            first_name: firstnameInputField?.text ?? null,
            last_name: surnameInputField?.text ?? null);
    }

    private void Start() {
        userWebManager.LoadUserInfo();
    }

    private void UserInfoLoadedCallBack() {
        if (usernameInputField != null)
            usernameInputField.text = usernameInputField.text == "" ? userWebManager.GetUsername() ?? "" : usernameInputField.text;

        if (firstnameInputField != null)
            firstnameInputField.text = firstnameInputField.text == "" ? userWebManager.GetFirstName() ?? "" : firstnameInputField.text;

        if (surnameInputField != null)
            surnameInputField.text = surnameInputField.text == "" ? userWebManager.GetLastName() ?? "" : surnameInputField.text;

        if ((userWebManager.GetUsername() == null && usernameInputField != null) ||
            userWebManager.GetFirstName() == null && firstnameInputField != null ||
            userWebManager.GetLastName() == null && surnameInputField != null) {
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

        if (badRequestData.username.Count > 0)
            usernameInputField.ShowWarning(badRequestData.username[0]);

        if (badRequestData.first_name.Count > 0)
            firstnameInputField.ShowWarning(badRequestData.first_name[0]);


        if (badRequestData.last_name.Count > 0)
            surnameInputField.ShowWarning(badRequestData.last_name[0]);

        if (!string.IsNullOrEmpty(badRequestData.detail))
            usernameInputField.ShowWarning(badRequestData.detail);
    }

    private void OnEnable() {
        userWebManager.OnUserInfoLoaded += UserInfoLoadedCallBack;
        userWebManager.OnUserInfoUploaded += UpdateUserDataCallBack;
        userWebManager.OnErrorUserUploaded += ErrorUpdateUserDataCallBack;

        InputDataArea.SetActive(false);
        userWebManager.LoadUserInfo();
    }

    private void OnDisable() {
        userWebManager.OnUserInfoLoaded -= UserInfoLoadedCallBack;
        userWebManager.OnUserInfoUploaded -= UpdateUserDataCallBack;
        userWebManager.OnErrorUserUploaded -= ErrorUpdateUserDataCallBack;

        foreach (var backBtn in backBtns) {
            backBtn.SetActive(false);
        }
    }
}
