using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PnlMenu : MonoBehaviour
{
    [SerializeField] UserWebManager userWebManager;
    [SerializeField] TMP_Text txtUsername;
    [SerializeField] GameObject goLiveBtn;

    private void UserInfoLoadedCallBack() {
        if (!this.isActiveAndEnabled)
            return;

        txtUsername.text = userWebManager.GetUsername();
        goLiveBtn.SetActive(userWebManager.IsBroadcaster());
    }

    private void OnEnable() {
        userWebManager.OnUserInfoLoaded += UserInfoLoadedCallBack;
        userWebManager.LoadUserInfo();

        txtUsername.text = userWebManager.GetUsername();
        goLiveBtn.SetActive(userWebManager.IsBroadcaster());
    }

    private void OnDisable() {
        userWebManager.OnUserInfoLoaded -= UserInfoLoadedCallBack;
    }
}
