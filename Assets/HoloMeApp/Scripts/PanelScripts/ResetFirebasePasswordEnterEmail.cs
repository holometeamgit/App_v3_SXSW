﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
public class ResetFirebasePasswordEnterEmail : MonoBehaviour {
    [SerializeField]
    InputFieldController emailInputField;
    [SerializeField]
    AuthController authController;
    [SerializeField]
    Switcher switchToLogIn;
    [SerializeField]
    bool needShowWarning = true;

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
        PnlGenericErrorConstructor.ActivateDoubleButton(null,
            string.Format("Changing a password associated with a Facebook account will create login issues with your Beem account."),
            "Continue",
            "Cancel",
            () => { PnlGenericErrorConstructor.Deactivate(); SendMsg(); },
            () => PnlGenericErrorConstructor.Deactivate(), true);
    }

    private void SendMsg() {
        CallBacks.onForgotAccount?.Invoke(emailInputField.text);
    }

    private void MsgSentCallBack() {
        PnlGenericErrorConstructor.ActivateSingleButton("Change password",
            string.Format("Password change information has been sent to email {0}", emailInputField.text),
            "Continue",
            () => { PnlGenericErrorConstructor.Deactivate(); switchToLogIn.Switch(); });
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

    private void OnEnable() {
        if (string.IsNullOrWhiteSpace(emailInputField.text))
            emailInputField.text = authController.GetEmail();


        CallBacks.onResetPasswordClick += SendMsgOnEmailForChangePassword;
        CallBacks.onFail += ErrorMsgCallBack;
        CallBacks.onResetPasswordMsgSent += MsgSentCallBack;
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
