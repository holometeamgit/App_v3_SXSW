using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
public class ResetFirebasePasswordEnterEmail : MonoBehaviour
{
    [SerializeField]
    PnlGenericError pnlGenericError;
    [SerializeField]
    InputFieldController emailInputField;
    [SerializeField]
    AuthController authController;
    [SerializeField]
    Switcher switchToLogIn;
    [SerializeField]
    bool needShowWarning = true;

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
        pnlGenericError.ActivateDoubleButton("",
            string.Format("Password change information has been sent to email {0}", emailInputField.text),
            "Continue",
            "Cancel",
            () => { pnlGenericError.gameObject.SetActive(false); SendMsg(); },
            () => pnlGenericError.gameObject.SetActive(false));
    }

    private void SendMsg() {
        CallBacks.onForgotAccount?.Invoke(emailInputField.text);
    }

    private void MsgSentCallBack() {
        pnlGenericError.ActivateSingleButton("Change password",
            string.Format("Password change information has been sent to email {0}", emailInputField.text),
            "Continue",
            () => { pnlGenericError.gameObject.SetActive(false); switchToLogIn.Switch(); });
    }

    private void ErrorMsgCallBack(string msg) {
        emailInputField.ShowWarning(msg);
    }

    private bool LocalDataVerification() {
        if (string.IsNullOrWhiteSpace(emailInputField.text))
            emailInputField.ShowWarning("This field is compulsory");

        return !string.IsNullOrWhiteSpace(emailInputField.text);
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
}
