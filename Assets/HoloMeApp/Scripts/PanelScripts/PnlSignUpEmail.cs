using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PnlSignUpEmail : MonoBehaviour {
    [SerializeField] EmailAccountManager emailAccountManager;
    [SerializeField] PnlEmailVerification pnlEmailVerification;
    [SerializeField] Switcher switcherToVerification;
    [SerializeField] InputFieldController inputFieldEmail;
    [SerializeField] InputFieldController inputFieldPassword;
    [SerializeField] InputFieldController inputFieldConfirmPassword;

    public void SignUp() {
        EmailSignUpJsonData emailSignUpJsonData = new EmailSignUpJsonData();
        emailSignUpJsonData.email = inputFieldEmail.text;
        emailSignUpJsonData.username = inputFieldEmail.text;
        emailSignUpJsonData.password1 = inputFieldPassword.text;
        emailSignUpJsonData.password2 = inputFieldConfirmPassword.text;

        if (LocalDataVerification())
            emailAccountManager.SignUp(emailSignUpJsonData);
    }

    public void ClearInputFieldData() {
        inputFieldEmail.text = "";
        inputFieldEmail.text = "";
        inputFieldPassword.text = "";
        inputFieldConfirmPassword.text = "";
    }

    private void SignUpCallBack() {
        switcherToVerification.Switch();
    }

    private void ErrorSignUpCallBack(BadRequestSignUpEmailJsonData badRequestData) {
        if (badRequestData == null ||
            (badRequestData.email.Count == 0 &&
            badRequestData.password1.Count == 0 &&
            badRequestData.password2.Count == 0 &&
            badRequestData.non_field_errors.Count == 0)) {
            inputFieldEmail.ShowWarning("Server Error " + badRequestData.code.ToString());
            return;
        }

        if (badRequestData.email.Count > 0)
            inputFieldEmail.ShowWarning(badRequestData.email[0]);
        if (badRequestData.password1.Count > 0)
            inputFieldPassword.ShowWarning(badRequestData.password1[0]);
        if (badRequestData.password2.Count > 0)
            inputFieldConfirmPassword.ShowWarning(badRequestData.password2[0]);
        if (badRequestData.non_field_errors.Count > 0)
            inputFieldConfirmPassword.ShowWarning(badRequestData.non_field_errors[0]);
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

    private void OnEnable() {
        emailAccountManager.OnSignUp += SignUpCallBack;
        emailAccountManager.OnErrorSignUp += ErrorSignUpCallBack;
    }

    private void OnDisable() {
        emailAccountManager.OnSignUp -= SignUpCallBack;
        emailAccountManager.OnErrorSignUp -= ErrorSignUpCallBack;
    }
}
