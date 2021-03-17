using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlLogIn : MonoBehaviour
{
    public delegate void SaveAccesseDelegate();

    [SerializeField] FacebookAccountManager facebookAccountManager;
    [SerializeField] Switcher switcherToMainMenu;
    [SerializeField] Switcher switcherToEmailLogin;

    private SaveAccesseDelegate saveAccesseDelegate;

    public void AppleSignUp() { }
    public void GoogleSignUp() { }

    public void FacebookLogIn() {
        saveAccesseDelegate = facebookAccountManager.SaveAccessTokens;
        facebookAccountManager.LogIn();
    }

    public void OpenMainPanel() {
        if (!gameObject.activeInHierarchy)
            return;
        if (saveAccesseDelegate != null)
            saveAccesseDelegate.Invoke();
        switcherToMainMenu.Switch();
    }
}
