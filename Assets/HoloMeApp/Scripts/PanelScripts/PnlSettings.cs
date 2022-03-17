using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using TMPro;
using Beem.Permissions;

/// <summary>
/// Settings Pnl
/// </summary>
public class PnlSettings : MonoBehaviour {
    [SerializeField]
    private GameObject _changePassword;
    [SerializeField]
    private TMP_Text _txtNickname;
    [Space]
    [SerializeField]
    private AccountManager _accountManager;
    [SerializeField]
    private UserWebManager _userWebManager;

    private void OnEnable() {
        _changePassword.SetActive(_accountManager.GetLogInType() == LogInType.Email);
        _txtNickname.text = _userWebManager.GetUsername();
        _userWebManager.OnUserAccountDeleted += UserLogOut;
    }

    /// <summary>
    /// Open Change Usename Window
    /// </summary>
    public void SettingsToChangeUserName() {
        ChangeUsernameConstructor.OnActivated?.Invoke(true);
    }

    /// <summary>
    /// Open Change Password Window
    /// </summary>
    public void SettingsToChangePassword() {
        ChangePasswordConstructor.OnActivated?.Invoke(true);
    }

    /// <summary>
    /// Open Delete Account Window
    /// </summary>
    public void SettingsToDeleteAccount() {
        WarningConstructor.ActivateDoubleButton("Delete account", "If you delete your account, you will lose\naccess to the Beem network. Are you sure\nyou want to continue?",
          "Cancel", "DELETE", null,
          () => {
              _userWebManager.DeleteUserAccount();
          });
    }

    private void UserLogOut() {
        WelcomeConstructor.OnActivated?.Invoke(true);
        GalleryConstructor.OnHide?.Invoke();
        SettingsConstructor.OnActivated?.Invoke(false);

        _accountManager.LogOut();
    }


    private void OnDisable() {
        _userWebManager.OnUserAccountDeleted -= UserLogOut;
    }

    /// <summary>
    /// Sign Up To Welcome
    /// </summary>
    public void SettingsToWelcome() {

        WarningConstructor.ActivateDoubleButton("Log Out", "Are you sure you want to log out?",
            "log out", "Cancel",
            () => {
                UserLogOut();
            });
    }
}
