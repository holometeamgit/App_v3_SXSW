using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Beem.Permissions;

public class PnlMenu : MonoBehaviour {
    [SerializeField] UserWebManager userWebManager;
    [SerializeField] GameObject goLiveBtn;
    [SerializeField] GameObject myRoomBtn;
    [SerializeField]
    private PermissionController _permissionController;

    private void UserInfoLoadedCallBack() {
        if (!this.isActiveAndEnabled)
            return;

        UpdateUI();
    }

    private void UpdateUI() {
        goLiveBtn.SetActive(userWebManager.CanGoLive());
        myRoomBtn.SetActive(userWebManager.CanStartRoom());
    }

    private void OnEnable() {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeySettingsPanel);
        userWebManager.OnUserInfoLoaded += UserInfoLoadedCallBack;
        userWebManager.LoadUserInfo();

        UpdateUI();
    }

    private void OnDisable() {
        userWebManager.OnUserInfoLoaded -= UserInfoLoadedCallBack;
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

    /// <summary>
    /// Open Room
    /// </summary>

    public void OpenRoom() {

        if (!_permissionController.CheckCameraMicAccess()) {
            return;
        }

        AnalyticsController.Instance.SendCustomEventToSpecifiedControllers(new AnalyticsLibraryAbstraction[] { AnalyticsCleverTapController.Instance, AnalyticsAmplitudeController.Instance }, AnalyticKeys.KeyGoLive, new Dictionary<string, string>() { { AnalyticParameters.ParamBroadcasterUserID, AnalyticsController.Instance.GetUserID } });


        SettingsConstructor.OnActivated?.Invoke(false);
        MenuConstructor.OnActivated?.Invoke(false);
        HomeScreenConstructor.OnActivated?.Invoke(false);
        StreamCallBacks.onCloseComments?.Invoke();
        StreamOverlayConstructor.onActivatedAsRoomBroadcaster?.Invoke(true);
    }

    /// <summary>
    /// Open Ar Messages
    /// </summary>
    public void OpenARMessages() {

        if (!_permissionController.CheckCameraMicAccess()) {
            return;
        }

    }

    /// <summary>
    /// Open Live Stream
    /// </summary>

    public void OpenLiveStream() {

        if (!_permissionController.CheckCameraMicAccess()) {
            return;
        }

        AnalyticsController.Instance.SendCustomEventToSpecifiedControllers(new AnalyticsLibraryAbstraction[] { AnalyticsCleverTapController.Instance, AnalyticsAmplitudeController.Instance }, AnalyticKeys.KeyGoLive, new Dictionary<string, string>() { { AnalyticParameters.ParamBroadcasterUserID, AnalyticsController.Instance.GetUserID } });


        SettingsConstructor.OnActivated?.Invoke(false);
        MenuConstructor.OnActivated?.Invoke(false);
        HomeScreenConstructor.OnActivated?.Invoke(false);
        StreamCallBacks.onCloseComments?.Invoke();
        StreamOverlayConstructor.onActivatedAsLiveBroadcaster?.Invoke(true);
    }
}
