using Beem.Permissions;
using Beem.UI;
using Firebase.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Btn for cell in AssetManagement
/// </summary>
public class CellBtn : MonoBehaviour,
    IARMsgDataView, IUserWebManagerView, IWebRequestHandlerView,
    IBusinessProfileManagerView,
    IPointerDownHandler, IPointerUpHandler {
    private enum State {
        Default,
        Tap
    }

    private ARMsgJSON.Data _arMsgData = default;
    private UserWebManager _userWebManager;
    private WebRequestHandler _webRequestHandler;
    private BusinessProfileManager _businessProfileManager;

    private Coroutine _tapTimerCoroutine;

    private const string TOPIC = "gallery_{0}";
    private const float LONG_CLICK_TIME = 0.314f;

    private const string BUSINESS_OPTIONS_VIEW = "BusinessOptionsView";

    SignalBus _signalBus;

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

    /// <summary>
    /// OpenUserARMsg
    /// </summary>
    public void OpenUserARMsg() {
        OpenARMsg();
    }

    public void OnPointerDown(PointerEventData eventData) {
        _tapTimerCoroutine = StartCoroutine(TapTimer());
    }

    public void OnPointerUp(PointerEventData eventData) {
        StopTimer();
    }

    private void Start() {
        _signalBus = FindObjectOfType<SignalBusMonoBehaviour>().SignalBus;
    }

    private bool CanShowPushNotificationPopup {
        get {
            return PlayerPrefs.GetInt("PushNotificationForARMessage" + _userWebManager?.GetUsername(), 1) == 1;
        }
        set {
            PlayerPrefs.SetInt("PushNotificationForARMessage" + _userWebManager?.GetUsername(), value ? 1 : 0);
        }
    }

    private void SuccessedBusinessProfile(BusinessProfileJsonData businessProfileData) {
        if (businessProfileData != null) {
            OpenBusinessOptions();
        } else {
            OpenARMsg();
        }
    }

    private void FailedBusinessProfile(WebRequestError error) {
        OpenARMsg();
    }

    private void OpenARMsg() {
        if (_arMsgData.GetStatus == ARMsgJSON.Data.PROCESSING_STATUS) {
            if (!CanShowPushNotificationPopup) {
                OpenProcessingPopup();
            } else {
                OpenNotificationPopup();
            }
        } else if (_arMsgData.GetStatus == ARMsgJSON.Data.COMPETED_STATUS) {
            ARMsgRecordConstructor.OnActivated?.Invoke(false);
            ARenaConstructor.onActivateForARMessaging?.Invoke(_arMsgData);
            ARMsgARenaConstructor.OnActivatedARena?.Invoke(_arMsgData);
            GalleryConstructor.OnHide?.Invoke();
            PnlRecord.CurrentUser = _arMsgData.user;
        }
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
        WarningConstructor.ActivateDoubleButton("Proccessing",
               "Your hologram is processing,\nwe can tell you when it's ready",
               "GOT IT!",
               "DELETE",
               () => {
                   FirebaseMessaging.SubscribeAsync(string.Format(TOPIC, _userWebManager?.GetUsername()));
                   CanShowPushNotificationPopup = false;
               }, () => { Delete(); });
    }

    private void Delete() {
        _signalBus.Fire(new DeleteARMsgSignal() { idARMsg = _arMsgData.id });
    }

    private void StopTimer() {
        if (_tapTimerCoroutine == null)
            return;

        StopCoroutine(_tapTimerCoroutine);
        _tapTimerCoroutine = null;
    }

    private void OnDisable() {
        StopTimer();
    }

    private IEnumerator TapTimer() {
        yield return new WaitForSeconds(LONG_CLICK_TIME);

        if (_arMsgData.ext_content_data == null || _arMsgData.ext_content_data.Count == 0 || _arMsgData.GetStatus == ARMsgJSON.Data.PROCESSING_STATUS) {
            OpenARMsg();
        } else {
            _businessProfileManager.GetMyData(SuccessedBusinessProfile, FailedBusinessProfile);
        }
    }
}