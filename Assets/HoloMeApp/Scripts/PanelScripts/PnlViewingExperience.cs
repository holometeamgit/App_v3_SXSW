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

    public void Show<T>(T data) {
        InitARena();

        if (data is StreamJsonData.Data) {
            if ((data as StreamJsonData.Data).HasStreamUrl) {
                ShowPrerecorded(data as StreamJsonData.Data);
            } else if ((data as StreamJsonData.Data).HasAgoraChannel) {
                ShowStadium(data as StreamJsonData.Data);
            }
        } else if (data is ARMsgJSON.Data) {
            ShowARMessaging(data as ARMsgJSON.Data);
        } else if (data is RoomJsonData) {
            ShowRoom(data as RoomJsonData);
        }

        CheckTutorialComplete();
    }

    /// <summary>
    /// Placement for Prerecorded
    /// </summary>
    /// <param name="data"></param>
    /// <param name="isTeaser"></param>
    private void ShowPrerecorded(StreamJsonData.Data data) {
        InitHologramVideo(data.CanGetTeaser ? data.teaser_s3_url : data.stream_s3_url);
        SendAnalyticsMessage("Prerecorded", data.user, data.id.ToString());
    }

    /// <summary>
    /// Placement for AR Messaging
    /// </summary>
    /// <param name="data"></param>
    private void ShowARMessaging(ARMsgJSON.Data data) {
        InitHologramVideo(data.ar_message_s3_link);
        SendAnalyticsMessage("AR Message", data.user, data.id.ToString());
    }

    /// <summary>
    /// Placement for Room
    /// </summary>
    /// <param name="data"></param>
    private void ShowRoom(RoomJsonData data) {
        InitHologramStream(data.agora_channel);
        SendAnalyticsMessage("Room", data.user, data.id.ToString());
    }

    /// <summary>
    /// Placement for Stadium
    /// </summary>
    /// <param name="data"></param>
    private void ShowStadium(StreamJsonData.Data data) {
        InitHologramStream(data.agora_channel);
        SendAnalyticsMessage("Stadium", data.user, data.id.ToString());
    }

    private void InitARena() {
        ApplicationSettingsHandler.Instance.ToggleSleepTimeout(true);
        ARController.onActivated?.Invoke(true);
        gameObject.SetActive(true);
    }

    private void SendAnalyticsMessage(string type, string username, string id) {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyStartPerformance, new System.Collections.Generic.Dictionary<string, string> { { AnalyticParameters.ParamEventName, $"{type}: {username}" }, { AnalyticParameters.ParamPerformanceID, id } });
    }

    private void CheckTutorialComplete() {
        if (tutorialState == TutorialState.TutorialComplete) {
            HideScanMessage();
        }
    }

    private void InitHologramVideo(string link) {
        _hologramHandler.InitSession();
        _hologramHandler.TogglePreRecordedVideoRenderer(true);
        _hologramHandler.PlayIfPlaced(link);
    }

    private void InitHologramStream(string agoraChannel) {
        _hologramHandler.InitSession();
        _hologramHandler.TogglePreRecordedVideoRenderer(false);
        _hologramHandler.AssignStreamName(agoraChannel);
        _hologramHandler.StartTrackingStream();
    }

    /// <summary>
    /// Hide Arena
    /// </summary>
    public void Hide() {
        ApplicationSettingsHandler.Instance.ToggleSleepTimeout(false);
        ARController.onActivated?.Invoke(false);
        _hologramHandler.StopVideo();
        HideScanAnimation();
        gameObject.SetActive(false);
    }
}