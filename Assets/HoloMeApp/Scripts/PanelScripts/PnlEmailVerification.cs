using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlEmailVerification : MonoBehaviour
{
    public delegate void SaveAccesseDelegate();

    [SerializeField] EmailAccountManager emailAccountManager;
    [SerializeField] InputFieldController inputFieldKey;
    [SerializeField] Switcher switcherToMainMenu;

    private SaveAccesseDelegate saveAccesseDelegate;

    public void SetActionOnVerified(SaveAccesseDelegate saveAccesseDelegate) {
        this.saveAccesseDelegate = saveAccesseDelegate;
    }

    public void Verify() {
        VerifyKeyJsonData verifyKeyJsonData = new VerifyKeyJsonData();
        verifyKeyJsonData.key = inputFieldKey.text;
        emailAccountManager.Verify(verifyKeyJsonData, EmailVerificationCallBack, ErrorEmailVerificationCallBack);
    }

    private void EmailVerificationCallBack(long code, string body) {
        //accountManager.SaveAccessToken(body);
        Debug.Log(code + " : " + body);
        saveAccesseDelegate.Invoke();
        switcherToMainMenu.Switch();
    }

    private void ErrorEmailVerificationCallBack(long code, string body) {
        Debug.Log(code + " : " + body);
        inputFieldKey.ShowWarning("Verification code does not match");
    }
}
