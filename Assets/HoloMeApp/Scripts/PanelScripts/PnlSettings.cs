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
    private GameObject _btnStadium;
    [SerializeField]
    private TMP_Text _txtNickname;
    [Space]
    [SerializeField]
    private AccountManager _accountManager;
    [SerializeField]
    private UserWebManager _userWebManager;
    [SerializeField]
    private PermissionController _permissionController;

    private void OnEnable() {
        _changePassword.SetActive(_accountManager.GetLogInType() == LogInType.Email);
        _txtNickname.text = _userWebManager.GetUsername();

        _btnStadium.SetActive(_userWebManager.CanGoLive());

        _userWebManager.OnUserAccountDeleted += UserLogOut;
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
        WarningConstructor.ActivateDoubleButton("Delete account", "If you delete your account, you will lose \naccess to the Beem network.Are you sure \nyou want to continue?",
          "Continue", "Cancel",
          () => {
              _userWebManager.DeleteUserAccount();
          });
    }

    private void UserLogOut() {
        WelcomeConstructor.OnActivated?.Invoke(true);
        MenuConstructor.OnActivated?.Invoke(false);
        HomeScreenConstructor.OnActivated?.Invoke(false);
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
