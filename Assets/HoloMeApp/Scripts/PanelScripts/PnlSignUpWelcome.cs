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
    [SerializeField] AnimatedTransition animatedTransition;


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
        animatedTransition.DoMenuTransition(false);
    }

    public void OpenLogInPanel() {
        switcherToLogIn.Switch();
        animatedTransition.DoMenuTransition(false);
    }

    public void OpenSignUpEmail() {
        switcherToSignUpEmail.Switch();
        animatedTransition.DoMenuTransition(false);
    }
}
    