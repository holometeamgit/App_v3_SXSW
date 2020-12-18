using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PnlProfile : MonoBehaviour {
    [SerializeField] UserWebManager userWebManager;
    [SerializeField] GameObject InputDataArea;
    [SerializeField] InputFieldController usernameInputField;
    [SerializeField] InputFieldController firstnameInputField;
    [SerializeField] InputFieldController surnameInputField;
    [SerializeField] int userNameLimit;
    [SerializeField] Switcher switchToMainMenu;
    [SerializeField] Switcher switchLogOut;

    [SerializeField] List<GameObject> backBtns;

    public void ChooseUsername() {
        if (LocalDataVerification())
            userWebManager.UpdateUserData(userName: usernameInputField?.text ?? null,
            first_name: firstnameInputField?.text ?? null,
            last_name: surnameInputField?.text ?? null);
    }

    private void Start() {
        usernameInputField.characterLimit = userNameLimit;
        userWebManager.LoadUserInfo();
    }

    private void UserInfoLoadedCallBack() {
        Debug.Log("UserInfoLoadedCallBack");
        Debug.Log(usernameInputField.text + " " + firstnameInputField.text + " " + surnameInputField.text);
        Debug.Log(userWebManager?.GetUsername() + " " + userWebManager?.GetFirstName() + " " + userWebManager?.GetLastName());
        usernameInputField.text = string.IsNullOrWhiteSpace(usernameInputField.text) ? userWebManager.GetUsername() ?? "" : usernameInputField.text;
        firstnameInputField.text = string.IsNullOrWhiteSpace(firstnameInputField.text) ? userWebManager.GetFirstName() ?? "" : firstnameInputField.text;
        surnameInputField.text = string.IsNullOrWhiteSpace(surnameInputField.text) ? userWebManager.GetLastName() ?? "" : surnameInputField.text;
        Debug.Log(usernameInputField.text + " " + firstnameInputField.text + " " + surnameInputField.text);
        if (userWebManager.GetUsername() == null ||
            userWebManager.GetFirstName() == null ||
            userWebManager.GetLastName() == null) {
            InputDataArea.SetActive(true);
        } else {
            SwitchToMainMenu();
        }
    }

    private void ErrorUserInfoLoadedCallBack() {
        switchLogOut?.Switch();
    }

    private void UpdateUserDataCallBack() {
        userWebManager.LoadUserInfo();
        SwitchToMainMenu();
    }

    private void SwitchToMainMenu() {
        switchToMainMenu.Switch();
    }

    private void ErrorUpdateUserDataCallBack(BadRequestUserUploadJsonData badRequestData) {

        if (badRequestData == null ||
            (string.IsNullOrEmpty(badRequestData.username) &&
            badRequestData.first_name.Count == 0 &&
            badRequestData.last_name.Count == 0 &&
            string.IsNullOrEmpty(badRequestData.detail))) {
            usernameInputField.ShowWarning("Server Error " + badRequestData.code.ToString());
            return;
        }

        if (!string.IsNullOrEmpty(badRequestData.username)) {
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
            usernameInputField.ShowWarning("This field is compulsory");
        else if (usernameInputField.text.Length > 20)
            usernameInputField.ShowWarning("Username must be 20 characters or less");
        if (string.IsNullOrWhiteSpace(firstnameInputField.text))
            firstnameInputField.ShowWarning("This field is compulsory");
        if (string.IsNullOrWhiteSpace(surnameInputField.text))
            surnameInputField.ShowWarning("This field is compulsory");

        return !string.IsNullOrWhiteSpace(usernameInputField.text) &&
            !string.IsNullOrWhiteSpace(firstnameInputField.text) &&
            !string.IsNullOrWhiteSpace(surnameInputField.text) &&
            usernameInputField.text.Length <= 20;
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

        ClearInputFieldData();

        foreach (var backBtn in backBtns) {
            backBtn.SetActive(false);
        }
    }
}
