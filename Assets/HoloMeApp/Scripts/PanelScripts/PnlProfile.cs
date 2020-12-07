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
    [SerializeField] Switcher switchLogOutToLogIn;

    [SerializeField] List<GameObject> backBtns;

    public void ChooseUsername() {
        if(LocalDataVerification())
            userWebManager.UpdateUserData(userName: usernameInputField?.text ?? null,
            first_name: firstnameInputField?.text ?? null,
            last_name: surnameInputField?.text ?? null);
    }

    private void Start() {
        userWebManager.LoadUserInfo();
    }

    private void UserInfoLoadedCallBack() {
        usernameInputField.text = usernameInputField.text == "" ? userWebManager.GetUsername() ?? "" : usernameInputField.text;
        firstnameInputField.text = firstnameInputField.text == "" ? userWebManager.GetFirstName() ?? "" : firstnameInputField.text;
        surnameInputField.text = surnameInputField.text == "" ? userWebManager.GetLastName() ?? "" : surnameInputField.text;

        if ((userWebManager.GetUsername() == null && usernameInputField != null) ||
            userWebManager.GetFirstName() == null && firstnameInputField != null ||
            userWebManager.GetLastName() == null && surnameInputField != null) {
            InputDataArea.SetActive(true);
        } else {
            SwitchToMainMenu();
        }
    }

    private void ErrorUserInfoLoadedCallBack() {
        switchLogOutToLogIn?.Switch();
    }

    private void UpdateUserDataCallBack() {
        userWebManager.LoadUserInfo();
        SwitchToMainMenu();
    }

    private void SwitchToMainMenu() {
        ClearInputFieldData();
        switchToMainMenu.Switch();
    }

    private void ErrorUpdateUserDataCallBack(BadRequestUserUploadJsonData badRequestData) {

        if (!string.IsNullOrEmpty(badRequestData.username)) {
            if(badRequestData.username.Contains("is exist"))
                usernameInputField.ShowWarning("Username already exists, please choose another");
            else
                usernameInputField.ShowWarning(badRequestData.username);
        }

        if (badRequestData.first_name.Count > 0)
            firstnameInputField.ShowWarning(badRequestData.first_name[0]);


        if (badRequestData.last_name.Count > 0)
            surnameInputField.ShowWarning(badRequestData.last_name[0]);

        if (!string.IsNullOrEmpty(badRequestData.detail))
            usernameInputField.ShowWarning(badRequestData.detail);
    }

    private void ClearInputFieldData() {
        usernameInputField.text = "";
        firstnameInputField.text = "";
        surnameInputField.text = "";
    }

    private bool LocalDataVerification() {
        if (string.IsNullOrWhiteSpace(usernameInputField.text))
            usernameInputField.ShowWarning("Field must be completed");
        if (string.IsNullOrWhiteSpace(firstnameInputField.text))
            firstnameInputField.ShowWarning("Field must be completed");
        if (string.IsNullOrWhiteSpace(surnameInputField.text))
            surnameInputField.ShowWarning("Field must be completed");

        return !string.IsNullOrWhiteSpace(usernameInputField.text) &&
            !string.IsNullOrWhiteSpace(firstnameInputField.text) &&
            !string.IsNullOrWhiteSpace(surnameInputField.text);
    }

    private void OnEnable() {
        userWebManager.OnUserInfoLoaded += UserInfoLoadedCallBack;
        userWebManager.OnErrorUserInfoLoaded += ErrorUserInfoLoadedCallBack;
        userWebManager.OnUserInfoUploaded += UpdateUserDataCallBack;
        userWebManager.OnErrorUserUploaded += ErrorUpdateUserDataCallBack;

        InputDataArea.SetActive(false);
        userWebManager.LoadUserInfo();
    }

    private void OnDisable() {
        userWebManager.OnUserInfoLoaded -= UserInfoLoadedCallBack;
        userWebManager.OnErrorUserInfoLoaded -= ErrorUserInfoLoadedCallBack;
        userWebManager.OnUserInfoUploaded -= UpdateUserDataCallBack;
        userWebManager.OnErrorUserUploaded -= ErrorUpdateUserDataCallBack;

        foreach (var backBtn in backBtns) {
            backBtn.SetActive(false);
        }
    }
}
