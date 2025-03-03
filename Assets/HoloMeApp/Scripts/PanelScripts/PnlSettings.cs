﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Beem.SSO;
using TMPro;
using Beem.Permissions;
using Zenject;

/// <summary>
/// Settings Pnl
/// </summary>
public class PnlSettings : MonoBehaviour {
    [SerializeField]
    private GameObject _changePassword;
    [SerializeField]
    private TMP_Text _txtNickname;

    [SerializeField]
    GameObject _businessGO;
    [SerializeField]
    private TMP_Text _txtBusinessName;
    [SerializeField]
    private GameObject _defaultBusinessLogoGO;
    [SerializeField]
    private Image _imgBusinessLogo;
    [SerializeField]
    GameObject _changeLogobtn;
    [SerializeField]
    private Sprite _defaultLogo;

    private UserWebManager _userWebManager;
    private AccountManager _accountManager;
    private BusinessProfileManager _businessProfileManager;

    [Inject]
    public void Construct(AccountManager accountManager, UserWebManager userWebManager, BusinessProfileManager businessProfileManager) {
        _accountManager = accountManager;
        _userWebManager = userWebManager;
        _businessProfileManager = businessProfileManager;
    }

    private void OnEnable() {
        CallBacks.onBusinessLogoUpdated += OnUpdateBusinessLogoImage;
        CallBacks.onBusinessDataUpdated += OnBusinessDataUpdated;

        _changePassword.SetActive(_accountManager.GetLogInType() == LogInType.Email);
        _txtNickname.text = _userWebManager.GetUsername();
        _userWebManager.OnUserAccountDeleted += UserLogOut;

        OnBusinessDataUpdated();
        CallBacks.onLoadLogo?.Invoke();
    }

    /// <summary>
    /// Open Change Usename Window
    /// </summary>
    public void SettingsToChangeUserName() {
        SettingsConstructor.OnActivated?.Invoke(false);
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

    private void UserLogOut() {
        WelcomeConstructor.OnActivated?.Invoke(true);
        GalleryConstructor.OnHide?.Invoke();
        SettingsConstructor.OnActivated?.Invoke(false);

        _defaultBusinessLogoGO.SetActive(true);
        _imgBusinessLogo.gameObject.SetActive(false);

        _accountManager.LogOut();
    }

    private void OnUpdateBusinessLogoImage() {
        _imgBusinessLogo.sprite = CallBacks.getLogoOnDevice();

        if (_imgBusinessLogo.sprite == null)
            _imgBusinessLogo.sprite = _defaultLogo;

        _imgBusinessLogo.preserveAspect = true;

        string businessName = _businessProfileManager.GetCTALable();
        _txtBusinessName.text = string.IsNullOrEmpty(businessName) ? "B" : businessName[0].ToString();
        _imgBusinessLogo.gameObject.SetActive(_imgBusinessLogo.sprite != null);
        _defaultBusinessLogoGO.SetActive(_imgBusinessLogo.sprite == null);
    }

    private void OnBusinessDataUpdated() {
        bool isBuisenessProfile = _businessProfileManager.IsBusinessProfile();
        _businessGO.SetActive(isBuisenessProfile);
        //_changeLogobtn.SetActive(isBuisenessProfile); //temporarily disable since 5.5.7

        if (!isBuisenessProfile)
            return;

        OnUpdateBusinessLogoImage();
    }


    private void OnDisable() {
        _userWebManager.OnUserAccountDeleted -= UserLogOut;
        CallBacks.onBusinessLogoUpdated -= OnUpdateBusinessLogoImage;
        CallBacks.onBusinessDataUpdated -= OnBusinessDataUpdated;
    }
}
