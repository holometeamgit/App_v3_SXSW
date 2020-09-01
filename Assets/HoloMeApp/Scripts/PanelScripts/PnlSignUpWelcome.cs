using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlSignUpWelcome : MonoBehaviour
{
    [SerializeField] FacebookAccountManager facebookAccountManager;
    [SerializeField] PnlProfile pnlProfile;
    [SerializeField] UserWebManager userWebManager;
    [SerializeField] Switcher switcherToProfile;
    [SerializeField] Switcher switcherToLogIn;
    [SerializeField] Switcher switcherToSignUpEmail;


    public void AppleSignUp() { }
    public void GoogleSignUp() { }

    public void FacebookSignUp() {
        facebookAccountManager.SignUp();
        pnlProfile.SetActionOnSignUp(facebookAccountManager.SaveAccessTokens);
    }

    public void OpenProfilePanel() {
        if (!gameObject.activeInHierarchy)
            return;


        switcherToProfile.Switch();
    }

    public void OpenLogInPanel() {
        switcherToLogIn.Switch();
    }

    public void OpenSignUpEmail() {
        switcherToSignUpEmail.Switch();
    }
}
    