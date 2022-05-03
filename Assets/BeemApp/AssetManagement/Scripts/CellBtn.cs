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
public class CellBtn : MonoBehaviour, IARMsgDataView, IUserWebManagerView, IWebRequestHandlerView, IBusinessProfileManagerView, IPointerDownHandler, IPointerUpHandler {
    private ARMsgJSON.Data _arMsgData = default;
    private UserWebManager _userWebManager;
    private WebRequestHandler _webRequestHandler;
    private BusinessProfileManager _businessProfileManager;
    private const string TOPIC = "gallery_{0}";
    private float currentTime;
    private const float LONG_CLICK_TIME = 0.15f;

    private const string BUSINESS_OPTIONS_VIEW = "BusinessOptionsView";

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

    public void Init(BusinessProfileManager businessProfileManager) {
        _businessProfileManager = businessProfileManager;
    }

    private void SuccessedBusinessProfile(BusinessProfileData businessProfileData) {
        OpenBusinessOptions();
    }

    private void FailedBusinessProfile(WebRequestError error) {
        OpenARMsg();
    }

    public void OnPointerUp(PointerEventData eventData) {

        if (_arMsgData.Status == ARMsgJSON.Data.COMPETED_STATUS) {
            if (Time.time - currentTime < LONG_CLICK_TIME) {
                OpenARMsg();
            } else {
                _businessProfileManager.GetMyData(SuccessedBusinessProfile, FailedBusinessProfile);
            }
        } else if (_arMsgData.Status == ARMsgJSON.Data.PROCESSING_STATUS) {
            if (!CanShowPushNotificationPopup) {
                OpenProcessingPopup();
            } else {
                OpenNotificationPopup();
            }
        }
    }

    private void OpenARMsg() {
        ARMsgRecordConstructor.OnActivated?.Invoke(false);
        ARenaConstructor.onActivateForARMessaging?.Invoke(_arMsgData);
        ARMsgARenaConstructor.OnActivatedARena?.Invoke(_arMsgData);
        GalleryConstructor.OnHide?.Invoke();
        PnlRecord.CurrentUser = _arMsgData.user;
    }

    private void OpenBusinessOptions() {
        BlindOptionsConstructor.Show(BUSINESS_OPTIONS_VIEW, _arMsgData, _userWebManager, _businessProfileManager, _webRequestHandler, true);
    }

    private void OpenNotificationPopup() {
        WarningConstructor.ActivateDoubleButton("Proccessing",
                       "Your hologram is processing,\nwe can tell you when it's ready",
                       "Turn on notifications",
                       "Close",
                       () => {
                           FirebaseMessaging.SubscribeAsync(string.Format(TOPIC, _userWebManager?.GetUsername()));
                           CanShowPushNotificationPopup = false;
                       });
    }

    private void OpenProcessingPopup() {
        WarningConstructor.ActivateSingleButton("Proccessing",
             "Your hologram is processing,\nwe can tell you when it's ready",
              "GOT IT!");
    }

    public void OnPointerDown(PointerEventData eventData) {
        currentTime = Time.time;
    }
}
