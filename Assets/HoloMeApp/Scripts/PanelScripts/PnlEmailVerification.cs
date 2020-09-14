using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PnlEmailVerification : MonoBehaviour {

    [SerializeField]
    EmailAccountManager emailAccountManager;
    [SerializeField]
    PnlSignUpEmail pnlSignUpEmail;
    [SerializeField]
    InputFieldController inputFieldKey;
    [SerializeField]
    DeepLinkHandler deepLinkHandler;
    [SerializeField]
    Switcher switcherToMainMenu;


    [SerializeField]
    GameObject ResendBtn;
    [SerializeField]
    TMP_Text txtVerificationInfo;
    [SerializeField]
    TMP_Text txtVerificationInfoResend;
    [SerializeField]
    TMP_Text txtEmail;

    public void Verify() {
        if (inputFieldKey == null)
            return;
        Verify(inputFieldKey.text);
    }

    public void ResendVerification() {
        pnlSignUpEmail?.SignUp();
        EnableVerificationInfo();
    }

    private void Verify(string key) {
        Debug.Log("Verify " + key);
        VerifyKeyJsonData verifyKeyJsonData = new VerifyKeyJsonData(key);
        emailAccountManager.Verify(verifyKeyJsonData);
    }

    private void EmailVerificationCallBack() {
        pnlSignUpEmail?.ClearInputFieldData();
        switcherToMainMenu.Switch();
    }

    private void ErrorEmailVerificationCallBack() {
        inputFieldKey?.ShowWarning("Verification code does not match");
        Debug.Log("Verification code does not match");

        EnableVerificationInfoResend();
    }

    private void EnableVerificationInfo() {
        txtVerificationInfo?.gameObject.SetActive(true);

        ResendBtn?.gameObject.SetActive(false);
        txtVerificationInfoResend?.gameObject.SetActive(false);

    }

    private void EnableVerificationInfoResend() {
        txtVerificationInfo?.gameObject.SetActive(false);

        ResendBtn?.gameObject.SetActive(true);
        txtVerificationInfoResend?.gameObject.SetActive(true);
    }

    private void OnEnable() {
        emailAccountManager.OnVerified += EmailVerificationCallBack;
        emailAccountManager.OnErrorVerification += ErrorEmailVerificationCallBack;

        if (txtEmail != null)
            txtEmail.text = emailAccountManager.GetLastSignUpEmail();

        EnableVerificationInfo();

        if (deepLinkHandler != null) //todo: delete in beem vertion
            deepLinkHandler.VerificationDeepLinkActivated += Verify;
    }

    private void OnDisable() {
        emailAccountManager.OnVerified -= EmailVerificationCallBack;
        emailAccountManager.OnErrorVerification -= ErrorEmailVerificationCallBack;

        if (deepLinkHandler != null) //todo: delete in beem vertion
            deepLinkHandler.VerificationDeepLinkActivated -= Verify;
    }
}
