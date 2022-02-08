using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Beem.Permissions;
using Beem.ARMsg;
using Zenject;

/// <summary>
/// Pnl for menu
/// </summary>
public class PnlMenu : MonoBehaviour {
    private UserWebManager _userWebManager;

    [Inject]
    public void Construct(UserWebManager userWebManager) {
        _userWebManager = userWebManager;
    }

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
        HomeConstructor.OnActivated?.Invoke(true);
        SettingsConstructor.OnActivated?.Invoke(false);
        StreamCallBacks.onCloseComments?.Invoke();
    }

    /// <summary>
    /// Open Settings
    /// </summary>
    public void OpenSettings() {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeySettingsPanel);
        SettingsConstructor.OnActivated?.Invoke(true);
        HomeConstructor.OnActivated?.Invoke(false);
        StreamCallBacks.onCloseComments?.Invoke();
    }
}
