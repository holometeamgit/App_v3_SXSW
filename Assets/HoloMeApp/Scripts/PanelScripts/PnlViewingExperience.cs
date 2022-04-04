using UnityEngine.XR.ARFoundation;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Collections;
using System;
using Beem.Permissions;
using Zenject;

public class PnlViewingExperience : MonoBehaviour {
    [SerializeField]
    GameObject scanAnimationItems;
    [SerializeField]
    CanvasGroup canvasGroup;
    [SerializeField]
    RectTransform scanMessageRT;
    [SerializeField]
    private bool skipTutorial;

    private HologramHandler _hologramHandler;

    Coroutine scanAnimationRoutine;
    bool tutorialDisplayed;
    float messageAnimationSpeed = 0.25f;
    float messageTime = 10;
    float animationSpeed = 0.25f;
    string scaneEnviromentStr = "Scan the floor in front of you by moving your phone slowly from side to side";

    string pinchToZoomStr = "Pinch to resize the hologram";

    string tapToPlaceStr = "To see your chosen performer, tap the white circle when it appears on the floor";
    private enum TutorialState { MessageScan, MessageTapToPlace, WaitingForTap, WaitingForPinch, TutorialComplete };
    TutorialState tutorialState = TutorialState.MessageScan;

    [Inject]
    public void Construct(HologramHandler hologramHandler) {
        _hologramHandler = hologramHandler;
    }

    void OnEnable() {
        scanAnimationItems.SetActive(false);
        if (!tutorialDisplayed) {
            tutorialDisplayed = true;
        }
    }

    public void RunTutorial() {
        if (!gameObject.activeSelf)
            return;
        if (tutorialState == TutorialState.TutorialComplete)
            return;
        if (scanAnimationRoutine != null)
            StopCoroutine(scanAnimationRoutine); //Stop old routine is reactivating
        if (Application.isEditor && skipTutorial) {
            tutorialState = TutorialState.WaitingForPinch;
            OnPlaced();
            return;
        }
        scanAnimationRoutine = StartCoroutine(StartScanAnimationLoop(messageTime));
        ShowMessage(scaneEnviromentStr);
        tutorialState = TutorialState.MessageTapToPlace;
        // arPlaneManager.enabled = true;
    }
    IEnumerator StartScanAnimationLoop(float toggleTime) {
        while (true) {
            float delay = toggleTime;
            if (scanAnimationItems.activeSelf) {
                delay = delay / 2; //half time before reshowing
                HideScanAnimation(animationSpeed);
            } else {
                ShowScanAnimation(animationSpeed);
            }
            yield return new WaitForSeconds(delay);
        }
    }
    private void ShowScanAnimation(float animationSpeed) {
        scanAnimationItems.transform.localScale = Vector3.zero;
        scanAnimationItems.SetActive(true);
        scanAnimationItems.transform.DOScale(Vector3.one, animationSpeed).SetDelay(0.5f);
    }
    private void HideScanAnimation(float animationSpeed = 0) {
        scanAnimationItems.transform.DOScale(Vector3.zero, animationSpeed).OnComplete(() => {
            scanAnimationItems.SetActive(false);
        });
    }
    public void ShowTapToPlaceMessage() {
        if (tutorialState == TutorialState.MessageTapToPlace) {
            StopCoroutine(scanAnimationRoutine);
            HideScanAnimation(animationSpeed);
            HideScanMessage();
            ShowMessage(tapToPlaceStr, messageAnimationSpeed);
            tutorialState = TutorialState.WaitingForTap;
        }
    }
    // TODO доделать сообщение
    public void ShowPinchToZoomMessage() {
        if (tutorialState == TutorialState.WaitingForTap) {
            HideScanMessage();
            ShowMessage(pinchToZoomStr, messageAnimationSpeed);
            tutorialState = TutorialState.WaitingForPinch;
        }
    }
    public void OnPlaced() {
        if (tutorialState == TutorialState.WaitingForPinch) {
            HideScanMessage();
            //StartCoroutine(DelayStartRecordPanel(messageAnimationSpeed, activatedForStreaming));
            tutorialState = TutorialState.TutorialComplete;
        }
    }
    public void ShowMessage(string message, float delay = 0) {
        scanMessageRT.localScale = Vector3.zero;
        scanMessageRT.GetComponentInChildren<TextMeshProUGUI>().text = message;
        //messageRT.DOAnchorPosY(messageRT.rect.height, messageAnimationSpeed).SetDelay(delay);
        scanMessageRT.DOScale(Vector3.one, animationSpeed).SetDelay(delay);
    }
    public void HideScanMessage() {
        //messageRT.DOAnchorPosY(0, messageAnimationSpeed);
        scanMessageRT.DOScale(Vector3.zero, animationSpeed).SetDelay(messageAnimationSpeed);
    }
    public void ActivateForPreRecorded(StreamJsonData.Data streamJsonData, bool isTeaser) {
        SharedActivationFunctions();
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyStartPerformance, new System.Collections.Generic.Dictionary<string, string> { { AnalyticParameters.ParamEventName, streamJsonData.title } });
        _hologramHandler.PlayIfPlaced(isTeaser ? streamJsonData.teaser_s3_url : streamJsonData.stream_s3_url, streamJsonData.user_id);
        _hologramHandler.TogglePreRecordedVideoRenderer(true);
        if (tutorialState == TutorialState.TutorialComplete) //Re-enable record settings if tutorial was complete when coming back to viewing
        {
            HideScanMessage();
        }
    }

    /// <summary>
    /// Placement for AR Messaging
    /// </summary>
    /// <param name="streamJsonData"></param>
    public void ActivateForARMessaging(ARMsgJSON.Data streamJsonData) {
        SharedActivationFunctions();
        _hologramHandler.PlayIfPlaced(streamJsonData.ar_message_s3_link);
        _hologramHandler.TogglePreRecordedVideoRenderer(true);
        if (tutorialState == TutorialState.TutorialComplete) //Re-enable record settings if tutorial was complete when coming back to viewing
        {
            HideScanMessage();
        }
    }

    public void ActivateForStreaming(string channelName, string streamID, bool isRoom) {
        SharedActivationFunctions();
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyStartPerformance, new System.Collections.Generic.Dictionary<string, string> { { AnalyticParameters.ParamEventName, "Live Stream: " + channelName }, { AnalyticParameters.ParamPerformanceID, streamID } });
        _hologramHandler.TogglePreRecordedVideoRenderer(false);
        _hologramHandler.AssignStreamName(channelName);
        _hologramHandler.StartTrackingStream();
        if (tutorialState == TutorialState.TutorialComplete) //Re-enable record settings if tutorial was complete when coming back to viewing
        {
            HideScanMessage();
        }
    }
    void SharedActivationFunctions() {
        ApplicationSettingsHandler.Instance.ToggleSleepTimeout(true);
        ARConstructor.onActivated?.Invoke(true);
        canvasGroup.alpha = 0;
        gameObject.SetActive(true);
        _hologramHandler.InitSession();
        FadeInCanvas();
    }
    void FadeInCanvas() {
        canvasGroup.DOFade(1, .5f);
    }
    public void FadeOutCanvas() {
        HideScanAnimation();
        canvasGroup.DOFade(0, .5f);
    }
    public void StopExperience() {
        ApplicationSettingsHandler.Instance.ToggleSleepTimeout(false);
        ARConstructor.onActivated?.Invoke(false);
        _hologramHandler.StopVideo();
        FadeOutCanvas();
    }
}