using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlLogInEmail : MonoBehaviour
{
    [SerializeField] AccountManager accountManager;
    [SerializeField] EmailAccountManager emailAccountManager;
    [SerializeField] InputFieldController inputFieldEmail;
    [SerializeField] InputFieldController inputFieldPassword;
    [SerializeField] Switcher switcherToMainMenu;

    public void LogIn() {
        EmailLogInJsonData emailLogInJsonData = new EmailLogInJsonData();

        emailLogInJsonData.username = inputFieldEmail.text;
        emailLogInJsonData.password = inputFieldPassword.text;

        emailAccountManager.LogIn(emailLogInJsonData, LogInCallBack, ErrorLogInCallBack);
    }

    private void LogInCallBack(long code, string body) {
        Debug.Log(code + " : " + body);
        accountManager.SaveAccessToken(body);
        switcherToMainMenu.Switch();
    }

    private void ErrorLogInCallBack(long code, string body) {
        Debug.Log(code + " : " + body);
        BadRequestLogInEmailJsonData badRequestData = JsonUtility.FromJson<BadRequestLogInEmailJsonData>(body);

        if (badRequestData.username.Count > 0)
            inputFieldEmail.ShowWarning(badRequestData.username[0]);
        //Debug.Log(badRequestData.username[0]);
        if (badRequestData.password.Count > 0)
            inputFieldPassword.ShowWarning(badRequestData.password[0]);
        //Debug.Log(badRequestData.email[0]);
        if(!string.IsNullOrEmpty(badRequestData.detail))
            inputFieldEmail.ShowWarning(badRequestData.detail);
    }
}
