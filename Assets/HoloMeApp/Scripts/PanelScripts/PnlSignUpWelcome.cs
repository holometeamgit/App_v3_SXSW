using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlSignUpWelcome : MonoBehaviour
{
    [SerializeField] FacebookAccountManager facebookAccountManager;
    [SerializeField] PnlProfile pnlProfile;
    [SerializeField] Switcher switcherToProfile;
    [SerializeField] Switcher switcherToLogIn;
    [SerializeField] Switcher switcherToSignUpEmail;


    public void AppleSignUp() { }
    public void GoogleSignUp() { }

    public void FacebookSignUp() {
        facebookAccountManager.SignUp();
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
    