using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using TMPro;

public class PnlSettings : MonoBehaviour {
    [SerializeField] GameObject changePassword;
    [SerializeField] AccountManager accountManager;
    [SerializeField] UserWebManager userWebManager;
    [SerializeField] TMP_Text txtNickname;

    private void OnEnable() {
        changePassword.SetActive(accountManager.GetLogInType() == LogInType.Email);
        txtNickname.text = userWebManager.GetUsername();
        userWebManager.OnUserAccountDeleted += UserLogOut;
    }

    /// <summary>
    /// Open Change Usename Window
    /// </summary>
    public void SettingsToChangeUserName() {
        ChangeUsernameConstructor.OnActivated?.Invoke(true);
        SettingsConstructor.OnActivated?.Invoke(false);
    }

    /// <summary>
    /// Open Change Password Window
    /// </summary>
    public void SettingsToChangePassword() {
        ChangePasswordConstructor.OnActivated?.Invoke(true);
        SettingsConstructor.OnActivated?.Invoke(false);
    }

    /// <summary>
    /// Open Delete Account Window
    /// </summary>
    public void SettingsToDeleteAccount() {
        GenericConstructor.ActivateDoubleButton("Delete account", "If you delete your account, you will lose \naccess to the Beem network.Are you sure \nyou want to continue?",
          "Continue", "Cancel",
          () => {
              userWebManager.DeleteUserAccount();
          });
    }

    private void UserLogOut() {
        WelcomeConstructor.OnActivated?.Invoke(true);
        MenuConstructor.OnActivated?.Invoke(false);
        HomeScreenConstructor.OnActivated?.Invoke(false);
        SettingsConstructor.OnActivated?.Invoke(false);
        accountManager.LogOut();
    }


    private void OnDisable() {
        userWebManager.OnUserAccountDeleted -= UserLogOut;
    }

    /// <summary>
    /// Sign Up To Welcome
    /// </summary>
    public void SettingsToWelcome() {

        GenericConstructor.ActivateDoubleButton("Log Out", "Are you sure you want to log out?",
            "log out", "Cancel",
            () => {
                UserLogOut();
            });
    }
}
