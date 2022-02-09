using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using Zenject;
using WindowManager.Extenject;

public class ResetFirebasePasswordEnterEmail : MonoBehaviour {
    [SerializeField]
    InputFieldController emailInputField;

    private AuthController _authController;

    [Inject]
    public void Construct(AuthController authController) {
        _authController = authController;
    }

    public void ForgotPasswordBtnClick() {
        CallBacks.onResetPasswordClick?.Invoke();
    }

    private void Awake() {
        CallBacks.onSignOut += ClearInputField;
    }

    private void SendMsgOnEmailForChangePassword() {
        if (!LocalDataVerification())
            return;

        SendMsg();
    }

    private void SendMsg() {
        CallBacks.onForgotAccount?.Invoke(emailInputField.text);
    }

    private void MsgSentCallBack() {
        emailInputField.MobileInputField.gameObject.SetActive(false);

        GeneralPopUpData.ButtonData closeButton = new GeneralPopUpData.ButtonData("Continue", () => { ResetPasswordToSignIn(); emailInputField.MobileInputField.gameObject.SetActive(true); });
        GeneralPopUpData data = new GeneralPopUpData("Change password", $"Password change information has been sent to email {emailInputField.text}", closeButton);

        WarningConstructor.OnShow?.Invoke(data);
    }

    private void ErrorMsgCallBack(string msg) {
        emailInputField.ShowWarning(msg);
    }

    private bool LocalDataVerification() {
        if (string.IsNullOrWhiteSpace(emailInputField.text))
            emailInputField.ShowWarning("This field is compulsory");

        return !string.IsNullOrWhiteSpace(emailInputField.text);
    }

    private void ClearInputField() {
        emailInputField.text = "";
    }

    /// <summary>
    /// Switch reset password to sign in
    /// </summary>
    public void ResetPasswordToSignIn() {
        ResetPasswordConstructor.OnActivated?.Invoke(false);
        SignInConstructor.OnActivated?.Invoke(true);
    }

    private void OnEnable() {
        if (string.IsNullOrWhiteSpace(emailInputField.text))
            emailInputField.text = _authController.GetEmail();


        CallBacks.onResetPasswordClick += SendMsgOnEmailForChangePassword;
        CallBacks.onFail += ErrorMsgCallBack;
        CallBacks.onResetPasswordMsgSent += MsgSentCallBack;
    }

    /// <summary>
    /// Back to settings
    /// </summary>
    public void ChangePasswordToSettings() {
        ChangePasswordConstructor.OnActivated?.Invoke(false);
        SettingsConstructor.OnActivated?.Invoke(true);
    }

    private void OnDisable() {
        CallBacks.onResetPasswordClick -= SendMsgOnEmailForChangePassword;
        CallBacks.onFail -= ErrorMsgCallBack;
        CallBacks.onResetPasswordMsgSent -= MsgSentCallBack;
    }

    private void OnDestroy() {
        CallBacks.onSignOut -= ClearInputField;
    }
}
