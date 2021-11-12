using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PnlMenu : MonoBehaviour {
    [SerializeField] UserWebManager userWebManager;
    [SerializeField] TMP_Text txtUsername;
    [SerializeField] GameObject goLiveBtn;
    [SerializeField] GameObject myRoomBtn;

    private void UserInfoLoadedCallBack() {
        if (!this.isActiveAndEnabled)
            return;

        UpdateUI();
    }

    private void UpdateUI() {
        if (txtUsername != null)
            txtUsername.text = userWebManager.GetUsername();
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
}
