using Beem.Permissions;
using Beem.UI;
using Firebase.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Btn for cell in AssetManagement
/// </summary>
public class CellBtn : MonoBehaviour, IARMsgDataView, IUserWebManager {

    private ARMsgJSON.Data _arMsgData = default;
    private UserWebManager _userWebManager;
    private const string TOPIC = "gallery_{0}";

    private bool CanShowPushNotificationPopup {
        get {
            return PlayerPrefs.GetInt("PushNotificationForARMessage" + _userWebManager?.GetUsername(), 1) == 1;
        }
        set {
            PlayerPrefs.SetInt("PushNotificationForARMessage" + _userWebManager?.GetUsername(), value ? 1 : 0);
        }
    }

    public void Init(ARMsgJSON.Data arMsgData) {
        _arMsgData = arMsgData;
    }

    public void Init(UserWebManager userWebManager) {
        _userWebManager = userWebManager;
    }

    /// <summary>
    /// Open AR Messages
    /// </summary>
    public void Open() {
        if (_arMsgData.processing_status == ARMsgJSON.Data.COMPETED_STATUS) {
            ARMsgRecordConstructor.OnActivated?.Invoke(false);
            ARenaConstructor.onActivateForARMessaging?.Invoke(_arMsgData);
            ARMsgARenaConstructor.OnActivatedARena?.Invoke(_arMsgData);
            GalleryConstructor.OnHide?.Invoke();
            PnlRecord.CurrentUser = _arMsgData.user;
        } else if (_arMsgData.processing_status == ARMsgJSON.Data.PROCESSING_STATUS) {
            if (!CanShowPushNotificationPopup) {
                WarningConstructor.ActivateSingleButton("Proccessing",
                "Your hologram is processing,\nwe can tell you when it's ready",
                 "GOT IT!");
            } else {
                WarningConstructor.ActivateDoubleButton("Proccessing",
                    "Your hologram is processing,\nwe can tell you when it's ready",
                    "Turn on notifications",
                    "Close",
                    () => {
                        FirebaseMessaging.SubscribeAsync(string.Format(TOPIC, _userWebManager?.GetUsername()));
                        CanShowPushNotificationPopup = false;
                    });
            }
        }
    }

}
