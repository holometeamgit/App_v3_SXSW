using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PnlMenu : MonoBehaviour
{
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
        txtUsername.text = userWebManager.GetUsername();
        goLiveBtn.SetActive(userWebManager.IsBroadcaster());
        myRoomBtn.SetActive(userWebManager.IsEnterpriseBroadcaster() || userWebManager.IsBroadcaster());
    }

    private void OnEnable() {
        userWebManager.OnUserInfoLoaded += UserInfoLoadedCallBack;
        userWebManager.LoadUserInfo();

        UpdateUI();
    }

    private void OnDisable() {
        userWebManager.OnUserInfoLoaded -= UserInfoLoadedCallBack;
    }
}
