using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using TMPro;
using Beem.Permissions;
using Zenject;
using WindowManager.Extenject;

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

    private AccountManager _accountManager;
    private UserWebManager _userWebManager;

    [Inject]
    public void Construct(UserWebManager userWebManager, AccountManager accountManager) {
        _userWebManager = userWebManager;
        _accountManager = accountManager;
    }

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

        GeneralPopUpData.ButtonData funcButton = new GeneralPopUpData.ButtonData("Continue", _userWebManager.DeleteUserAccount);
        GeneralPopUpData.ButtonData closeButton = new GeneralPopUpData.ButtonData("Cancel", null);
        GeneralPopUpData data = new GeneralPopUpData("Delete account", "If you delete your account, you will lose \naccess to the Beem network.Are you sure \nyou want to continue?", closeButton, funcButton);

        WarningConstructor.OnShow?.Invoke(data);
    }

    private void UserLogOut() {
        WelcomeConstructor.OnActivated?.Invoke(true);
        HomeConstructor.OnActivated?.Invoke(false);
        BottomMenuConstructor.OnActivated?.Invoke(false);
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

        GeneralPopUpData.ButtonData funcButton = new GeneralPopUpData.ButtonData("Log Out", UserLogOut);
        GeneralPopUpData.ButtonData closeButton = new GeneralPopUpData.ButtonData("Cancel", null);
        GeneralPopUpData data = new GeneralPopUpData(null, "Are you sure you want to log out?", closeButton, funcButton);

        WarningConstructor.OnShow?.Invoke(data);
    }
}
