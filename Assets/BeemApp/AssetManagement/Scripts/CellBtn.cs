using Beem.Permissions;
using Beem.UI;
using Firebase.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Btn for cell in AssetManagement
/// </summary>
public class CellBtn : MonoBehaviour, IARMsgDataView, IUserWebManager, IWebRequestHandlerView, IPointerDownHandler, IPointerUpHandler {

    [SerializeField]
    private string _bussinessOptionsId;

    private ARMsgJSON.Data _arMsgData = default;
    private UserWebManager _userWebManager;
    private WebRequestHandler _webRequestHandler;
    private const string TOPIC = "gallery_{0}";
    private float currentTime;
    private const float LONG_CLICK_TIME = 0.15f;

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

    public void Init(WebRequestHandler webRequestHandler) {
        _webRequestHandler = webRequestHandler;
    }

    public void OnPointerUp(PointerEventData eventData) {

        if (_arMsgData.Status == ARMsgJSON.Data.COMPETED_STATUS) {
            if (IsBusinessProfile) {
                if (Time.time - currentTime < LONG_CLICK_TIME) {
                    ARMsgRecordConstructor.OnActivated?.Invoke(false);
                    ARenaConstructor.onActivateForARMessaging?.Invoke(_arMsgData);
                    ARMsgARenaConstructor.OnActivatedARena?.Invoke(_arMsgData);
                    GalleryConstructor.OnHide?.Invoke();
                    PnlRecord.CurrentUser = _arMsgData.user;
                } else {
                    BlindOptionsConstructor.OnShow?.Invoke(_bussinessOptionsId, new object[] { _arMsgData, _userWebManager, _webRequestHandler, true });
                }
            } else {
                ARMsgRecordConstructor.OnActivated?.Invoke(false);
                ARenaConstructor.onActivateForARMessaging?.Invoke(_arMsgData);
                ARMsgARenaConstructor.OnActivatedARena?.Invoke(_arMsgData);
                GalleryConstructor.OnHide?.Invoke();
                PnlRecord.CurrentUser = _arMsgData.user;
            }
        } else if (_arMsgData.Status == ARMsgJSON.Data.PROCESSING_STATUS) {
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

    private bool IsBusinessProfile {
        get {
            return true;
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        currentTime = Time.time;
    }
}
