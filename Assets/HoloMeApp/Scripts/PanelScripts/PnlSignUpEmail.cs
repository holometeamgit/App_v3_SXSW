using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PnlSignUpEmail : MonoBehaviour {
    [SerializeField] EmailAccountManager emailAccountManager;
    [SerializeField] AccountManager accountManager;
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
        //if (badRequestData.username.Count > 0)
        //  inputFieldFullName.ShowWarning(badRequestData.username[0]);
        //Debug.Log(badRequestData.username[0]);
        if (badRequestData.email.Count > 0)
            inputFieldEmail.ShowWarning(badRequestData.email[0]);
        if (badRequestData.password1.Count > 0)
            inputFieldPassword.ShowWarning(badRequestData.password1[0]);
        if (badRequestData.non_field_errors.Count > 0)
            inputFieldConfirmPassword.ShowWarning(badRequestData.non_field_errors[0]);
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
