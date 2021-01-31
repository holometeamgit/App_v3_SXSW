using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;

public class PnlSignUpEmailFirebase : MonoBehaviour
{
    [SerializeField] InputFieldController inputFieldEmail;
    [SerializeField] InputFieldController inputFieldPassword;
    [SerializeField] InputFieldController inputFieldConfirmPassword;
    [SerializeField] Switcher switcherToVerification;

    private const float COOLDOWN = 0.5f;
    private float nextTimeCanClick = 0;

    private void Awake() {
        CallBacks.onSignInSuccess += ClearInputFieldData;
    }

    private void SignUp() {
        HelperFunctions.DevLog("Start SignUp");
        if (Time.time < nextTimeCanClick)
            return;
        nextTimeCanClick = (Time.time + COOLDOWN);

        if (!LocalDataVerification())
            return;
        CallBacks.onSignUp?.Invoke(inputFieldEmail.text,
            inputFieldEmail.text,
            inputFieldPassword.text,
            inputFieldConfirmPassword.text);
    }

    private void SignUpCallBack() {
        switcherToVerification.Switch();
        ClearInputFieldData();
    }

    private void ErrorSignUpCallBack(string msg) {
        if(msg == "Passwords do not match") {
            inputFieldPassword.ShowWarning(msg);
            inputFieldConfirmPassword.ShowWarning(msg);
        }
        inputFieldEmail.ShowWarning(msg);
    }

    private bool LocalDataVerification() {
        if (string.IsNullOrWhiteSpace(inputFieldEmail.text))
            inputFieldEmail.ShowWarning("This field is compulsory");
        if (string.IsNullOrWhiteSpace(inputFieldPassword.text))
            inputFieldPassword.ShowWarning("This field is compulsory");
        if (string.IsNullOrWhiteSpace(inputFieldConfirmPassword.text))
            inputFieldConfirmPassword.ShowWarning("This field is compulsory");

        return !string.IsNullOrWhiteSpace(inputFieldEmail.text) &&
            !string.IsNullOrWhiteSpace(inputFieldPassword.text) &&
            !string.IsNullOrWhiteSpace(inputFieldConfirmPassword.text);
    }

    private void ClearInputFieldData() {
        inputFieldEmail.text = "";
        inputFieldPassword.text = "";
        inputFieldConfirmPassword.text = "";
    }

    private void OnEnable() {
        CallBacks.onSignUpEMailClick += SignUp;
        CallBacks.onFail += ErrorSignUpCallBack;
        CallBacks.onSignUpSuccess += SignUpCallBack;
    }

    private void OnDisable() {
        CallBacks.onSignUpEMailClick -= SignUp;
        CallBacks.onFail -= ErrorSignUpCallBack;
        CallBacks.onSignUpSuccess -= SignUpCallBack;
    }
}
