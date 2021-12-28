using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Beem.Permissions;
using Beem.ARMsg;

/// <summary>
/// Pnl for menu
/// </summary>
public class PnlMenu : MonoBehaviour {
    [SerializeField]
    private UserWebManager _userWebManager;
    [SerializeField]
    private PermissionController _permissionController;

    private void UserInfoLoadedCallBack() {
        if (!this.isActiveAndEnabled)
            return;

        UpdateUI();
    }

    private void UpdateUI() {
        ///TODO: Disable/Enable Live/Room Buttons
        //goLiveBtn.SetActive(_userWebManager.CanGoLive());
        //myRoomBtn.SetActive(_userWebManager.CanStartRoom());
    }

    private void OnEnable() {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeySettingsPanel);
        _userWebManager.OnUserInfoLoaded += UserInfoLoadedCallBack;
        _userWebManager.LoadUserInfo();

        UpdateUI();
    }

    private void OnDisable() {
        _userWebManager.OnUserInfoLoaded -= UserInfoLoadedCallBack;
    }

    /// <summary>
    /// Open Main Menu
    /// </summary>
    public void OpenMenu() {
        MenuConstructor.OnActivated?.Invoke(true);
        SettingsConstructor.OnActivated?.Invoke(false);
        StreamCallBacks.onCloseComments?.Invoke();
    }

    /// <summary>
    /// Open Settings
    /// </summary>
    public void OpenSettings() {
        SettingsConstructor.OnActivated?.Invoke(true);
        MenuConstructor.OnActivated?.Invoke(false);
        StreamCallBacks.onCloseComments?.Invoke();
    }
}
