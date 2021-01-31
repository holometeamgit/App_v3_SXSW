using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;


public class PnlLogInEmailFirebase : MonoBehaviour {
    [SerializeField] PnlGenericError pnlGenericError;
    [SerializeField] InputFieldController inputFieldEmail;
    [SerializeField] InputFieldController inputFieldPassword;
    [SerializeField] Switcher switcherToProfile;

    private const float COOLDOWN = 0.5f;
    private float nextTimeCanClick = 0;

    public void LogIn() {
        if (Time.time < nextTimeCanClick)
            return;
        nextTimeCanClick += (Time.time + COOLDOWN);

        if (!LocalDataVerification())
            return;
        CallBacks.onSignInEMail?.Invoke(inputFieldEmail.text, inputFieldPassword.text);
    }

    private void LogInCallBack() {
        ClearData();
        switcherToProfile.Switch();
    }

    private void ErrorLogInCallBack(string msg) {

        if (msg.Contains("WrongPassword")) {
            inputFieldPassword.ShowWarning(msg);
        } else {
            inputFieldEmail.ShowWarning(msg);
        }
    }

    private bool LocalDataVerification() {
        if (string.IsNullOrWhiteSpace(inputFieldEmail.text))
            inputFieldEmail.ShowWarning("This field is compulsory");
        if (string.IsNullOrWhiteSpace(inputFieldPassword.text))
            inputFieldPassword.ShowWarning("This field is compulsory");

        return !string.IsNullOrWhiteSpace(inputFieldEmail.text) &&
            !string.IsNullOrWhiteSpace(inputFieldPassword.text);
    }

    private void NeedVerificationCallback(string email) {
        inputFieldEmail.ShowWarning("E-mail is not verified");

        pnlGenericError.ActivateDoubleButton("Email verication",
            string.Format("You have not activated your account via the email, would you like us to send it again? {0}", email),
            "Yes",
            "No",
            () => {
                CallBacks.onRequestRepeatVerification?.Invoke();
            },
            () => { pnlGenericError.gameObject.SetActive(false); });
    }

    private void ClearData() {
        inputFieldEmail.SetToDefaultState();
        inputFieldEmail.text = "";
        inputFieldPassword.SetToDefaultState();
        inputFieldPassword.text = "";
    }

    private void OnEnable() {
        CallBacks.onSignInEMailClick += LogIn;
        CallBacks.onFail += ErrorLogInCallBack;
        CallBacks.onNeedVerification += NeedVerificationCallback;
        CallBacks.onSignInSuccess += LogInCallBack;
    }

    private void OnDisable() {
        CallBacks.onSignInEMailClick -= LogIn;
        CallBacks.onFail -= ErrorLogInCallBack;
        CallBacks.onNeedVerification -= NeedVerificationCallback;
        CallBacks.onSignInSuccess -= LogInCallBack;
    }
}
