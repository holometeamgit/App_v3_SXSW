using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Beem.SSO;

public class PnlProfile : MonoBehaviour {
    [SerializeField] UserWebManager userWebManager;
    [SerializeField] GameObject InputDataArea;
    [SerializeField] InputFieldController usernameInputField;
    [SerializeField] int userNameLimit;
    [SerializeField] Switcher switchToMainMenu;
    [SerializeField] Switcher switchLogOut;

    [SerializeField] List<GameObject> backBtns;

    [SerializeField] PnlGenericError pnlGenericError;
    [SerializeField] ExternalLinkRedirector externalLinkRedirector;

    [SerializeField]
    Toggle isPolicyConfirmed;
    [SerializeField]
    Button continueBtn;


    public void OnPolicyConfirmationChanged(bool isPolicyConfirmed) {
        continueBtn.interactable = isPolicyConfirmed;
    }

    public void ChooseUsername() {
        if (LocalDataVerification())
            userWebManager.UpdateUserData(userName: usernameInputField.text);
    }

    private void Start() {
        usernameInputField.characterLimit = userNameLimit;
        userWebManager.LoadUserInfo();
    }

    private void UserInfoLoadedCallBack() {
        usernameInputField.text = string.IsNullOrWhiteSpace(usernameInputField.text) ? userWebManager.GetUsername() ?? "" : usernameInputField.text;
        if (userWebManager.GetUsername() == null) {
            InputDataArea.SetActive(true);
        } else {
            SwitchToMainMenu();
        }
    }

    private void ErrorUserInfoLoadedCallBack() {
        ShowMsgForDeletedUser();
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

        if (!string.IsNullOrEmpty(badRequestData.detail))
            usernameInputField.ShowWarning(badRequestData.detail);
    }

    private void ClearInputFieldData() {
        usernameInputField.text = "";
    }

    private bool LocalDataVerification() {
        if (string.IsNullOrWhiteSpace(usernameInputField.text))
            usernameInputField.ShowWarning("This field is compulsory");
        else if (usernameInputField.text.Length > 20)
            usernameInputField.ShowWarning("Username must be 20 characters or less");

        return !string.IsNullOrWhiteSpace(usernameInputField.text) &&
            usernameInputField.text.Length <= 20;
    }

    private void ShowMsgForDeletedUser() {
        pnlGenericError.ActivateDoubleButton(null,
            string.Format("This account has been deleted, contact support to reinstate. "),
            "Support",
            "Cancel",
            () => { externalLinkRedirector.Redirect(); });
    }

    private void OnEnable() {
        userWebManager.OnUserInfoLoaded += UserInfoLoadedCallBack;
        userWebManager.OnErrorUserInfoLoaded += ErrorUserInfoLoadedCallBack;
        userWebManager.OnUserInfoUploaded += UpdateUserDataCallBack;
        userWebManager.OnErrorUserUploaded += ErrorUpdateUserDataCallBack;

        InputDataArea.SetActive(false);
        userWebManager.LoadUserInfo();

        isPolicyConfirmed.isOn = true;
        isPolicyConfirmed.enabled = false;
        isPolicyConfirmed.enabled = true;

        continueBtn.interactable = isPolicyConfirmed.isOn;
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
