using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using Zenject;

public class ResetFirebasePasswordEnterEmail : MonoBehaviour {
    [SerializeField]
    InputFieldController emailInputField;

    [SerializeField]
    private bool needShowWarning = true;

    private AccountManager _accountManager;
    private AuthController _authController;

    [Inject]
    public void Construct(AccountManager accountManager, AuthController authController) {
        _accountManager = accountManager;
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

        if (needShowWarning) {
            ShowWarning();
        } else {
            SendMsg();
        }
    }

    private void ShowWarning() {
        emailInputField.MobileInputField?.gameObject.SetActive(false);
        WarningConstructor.ActivateDoubleButton(null,
            string.Format("Changing a password associated with a Facebook account will create login issues with your Beem account."),
            "Continue",
            "Cancel",
            () => { SendMsg(); emailInputField.MobileInputField.gameObject.SetActive(true); },
            () => { emailInputField.MobileInputField.gameObject.SetActive(true); }, true);
    }

    private void SendMsg() {
        CallBacks.onForgotAccount?.Invoke(emailInputField.text);
    }

    private void MsgSentCallBack() {
        emailInputField.MobileInputField?.gameObject.SetActive(false);
        WarningConstructor.ActivateSingleButton("Change password",
            string.Format("Password change information has been sent to email {0}", emailInputField.text),
            "Continue",
            () => { ResetPasswordToSignIn(); });
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
        ChangePasswordConstructor.OnActivated?.Invoke(false);
        SettingsConstructor.OnActivated?.Invoke(false);
        GalleryConstructor.OnHide?.Invoke();
        ResetPasswordConstructor.OnActivated?.Invoke(false);
        SignInConstructor.OnActivated?.Invoke(true);
        _accountManager.LogOut();

    }

    private void OnEnable() {
        emailInputField.MobileInputField.gameObject.SetActive(true);
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
