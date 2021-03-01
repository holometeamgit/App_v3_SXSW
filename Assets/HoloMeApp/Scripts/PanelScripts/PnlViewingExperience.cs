using UnityEngine.XR.ARFoundation;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Collections;
using System;
public class PnlViewingExperience : MonoBehaviour
{
    [SerializeField]
    GameObject scanAnimationItems;
    [SerializeField]
    GameObject btnBurger;
    [SerializeField]
    CanvasGroup canvasGroup;
    [SerializeField]
    PermissionController permissionController;
    [SerializeField]
    HologramHandler hologramHandler;
    [SerializeField]
    BlurController blurController;
    [SerializeField]
    PermissionGranter permissionGranter;
    [SerializeField]
    ARPlaneManager arPlaneManager;
    [SerializeField]
    LogoCanvas logoCanvas;
    [SerializeField]
    PnlRecord pnlRecord;
    [SerializeField]
    GameObject arSessionOrigin;
    [SerializeField]
    GameObject arSession;
    [Header("")]
    [SerializeField]
    RectTransform scanMessageRT;
    //    [TextArea]
    //    [SerializeField]
    string scaneEnviromentStr = "Scan the floor in front of you by moving your phone slowly from side to side";
    //    [TextArea]
    //    [SerializeField]
    string pinchToZoomStr = "Pinch to resize the hologram";
    //    [TextArea]
    //    [SerializeField]
    string tapToPlaceStr = "To see your chosen performer, tap the white circle when it appears on the floor";
    Coroutine scanAnimationRoutine;
    [SerializeField]
    bool skipTutorial;
    bool activatedForStreaming;
    bool isTeaser;
    bool viewingExperienceInFocus;
    bool tutorialDisplayed;
    float messageAnimationSpeed = 0.25f;
    float messageTime = 10;
    float animationSpeed = 0.25f;
    private enum TutorialState { MessageScan, MessageTapToPlace, WaitingForTap, WaitingForPinch, TutorialComplete };
    TutorialState tutorialState = TutorialState.MessageScan;
    void OnEnable()
    {
        scanAnimationItems.SetActive(false);
        if (permissionGranter.HasCameraAccess && !tutorialDisplayed)
        {
            //RunTutorial();
            tutorialDisplayed = true;
        }
        else
        {
            if(permissionController == null)
                permissionController = FindObjectOfType<PermissionController>();
            permissionController.CheckCameraMicAccess();
        }
    }
    private void ToggleARSessionObjects(bool enable)
    {
        arSessionOrigin?.SetActive(enable);
        arSession?.SetActive(enable);
    }
    public void RunTutorial()
    {
        if (!gameObject.activeSelf)
            return;
        if (tutorialState == TutorialState.TutorialComplete)
            return;
        if (scanAnimationRoutine != null)
            StopCoroutine(scanAnimationRoutine); //Stop old routine is reactivating
        if (Application.isEditor && skipTutorial)
        {
            tutorialState = TutorialState.WaitingForPinch;
            OnPlaced();
            return;
        }
        hologramHandler.SetOnPlacementUIHelperFinished(() => StartCoroutine(DelayStartRecordPanel(messageAnimationSpeed, activatedForStreaming)));
        scanAnimationRoutine = StartCoroutine(StartScanAnimationLoop(messageTime));
        ShowMessage(scaneEnviromentStr);
        tutorialState = TutorialState.MessageTapToPlace;
        // arPlaneManager.enabled = true;
    }
    IEnumerator StartScanAnimationLoop(float toggleTime)
    {
        while (true)
        {
            float delay = toggleTime;
            if (scanAnimationItems.activeSelf)
            {
                delay = delay / 2; //half time before reshowing
                HideScanAnimation(animationSpeed);
            }
            else
            {
                ShowScanAnimation(animationSpeed);
            }
            yield return new WaitForSeconds(delay);
        }
    }
    private void ShowScanAnimation(float animationSpeed)
    {
        scanAnimationItems.transform.localScale = Vector3.zero;
        scanAnimationItems.SetActive(true);
        scanAnimationItems.transform.DOScale(Vector3.one, animationSpeed).SetDelay(0.5f);
    }
    private void HideScanAnimation(float animationSpeed)
    {
        scanAnimationItems.transform.DOScale(Vector3.zero, animationSpeed).OnComplete(() =>
        {
            scanAnimationItems.SetActive(false);
        });
    }
    public void ShowTapToPlaceMessage()
    {
        if (tutorialState == TutorialState.MessageTapToPlace)
        {
            StopCoroutine(scanAnimationRoutine);
            HideScanAnimation(animationSpeed);
            HideScanMessage();
            ShowMessage(tapToPlaceStr, messageAnimationSpeed);
            tutorialState = TutorialState.WaitingForTap;
        }
    }
    // TODO доделать сообщение
    public void ShowPinchToZoomMessage()
    {
        if (tutorialState == TutorialState.WaitingForTap)
        {
            HideScanMessage();
            ShowMessage(pinchToZoomStr, messageAnimationSpeed);
            tutorialState = TutorialState.WaitingForPinch;
        }
    }
    public void OnPlaced()
    {
        if (tutorialState == TutorialState.WaitingForPinch)
        {
            HideScanMessage();
            //StartCoroutine(DelayStartRecordPanel(messageAnimationSpeed, activatedForStreaming));
            tutorialState = TutorialState.TutorialComplete;
        }
    }
    IEnumerator DelayStartRecordPanel(float delay, bool streamPanel)
    {
        yield return new WaitForSeconds(delay);
        pnlRecord.EnableRecordPanel(isTeaser, streamPanel);
    }
    public void ShowMessage(string message, float delay = 0)
    {
        scanMessageRT.localScale = Vector3.zero;
        scanMessageRT.GetComponentInChildren<TextMeshProUGUI>().text = message;
        //messageRT.DOAnchorPosY(messageRT.rect.height, messageAnimationSpeed).SetDelay(delay);
        scanMessageRT.DOScale(Vector3.one, animationSpeed).SetDelay(delay);
    }
    public void HideScanMessage()
    {
        //messageRT.DOAnchorPosY(0, messageAnimationSpeed);
        scanMessageRT.DOScale(Vector3.zero, animationSpeed).SetDelay(messageAnimationSpeed);
    }
    public void ActivateForPreRecorded(string code, VideoJsonData videoJsonData, bool isTeaser)
    {
        //print($"PLAY CALLED - " + code);
        SharedActivationFunctions();
        this.isTeaser = isTeaser;
        activatedForStreaming = false;
        btnBurger.SetActive(true);
        logoCanvas.ActivateIfLogoAvailable(videoJsonData);
        hologramHandler.PlayIfPlaced(code);
        hologramHandler.TogglePreRecordedVideoRenderer(true);
        if (tutorialState == TutorialState.TutorialComplete) //Re-enable record settings if tutorial was complete when coming back to viewing
        {
            HideScanMessage();
            StartCoroutine(DelayStartRecordPanel(messageAnimationSpeed, false));
        }
    }
    public void ActivateForStreaming(string channelName)
    {
        StopExperience();
        SharedActivationFunctions();
        this.isTeaser = false;
        activatedForStreaming = true;
        btnBurger.SetActive(false); //Close button not required on this page
        hologramHandler.TogglePreRecordedVideoRenderer(false);
        hologramHandler.AssignStreamName(channelName + DateTime.Now);
        hologramHandler.StartTrackingStream();
        if (tutorialState == TutorialState.TutorialComplete) //Re-enable record settings if tutorial was complete when coming back to viewing
        {
            HideScanMessage();
            StartCoroutine(DelayStartRecordPanel(messageAnimationSpeed, true));
        }
    }
    void SharedActivationFunctions()
    {
        ApplicationSettingsHandler.Instance.ToggleSleepTimeout(true);
        ToggleARSessionObjects(true);
        blurController.RemoveBlur();
        canvasGroup.alpha = 0;
        gameObject.SetActive(true);
        hologramHandler.InitSession();
        FadeInCanvas();
        viewingExperienceInFocus = true;
    }
    void FadeInCanvas()
    {
        canvasGroup.DOFade(1, .5f);
    }
    public void FadeOutCanvas()
    {
        canvasGroup.DOFade(0, .5f);
    }
    public void StopExperience()
    {
        ApplicationSettingsHandler.Instance.ToggleSleepTimeout(false);
        ToggleARSessionObjects(false);
        hologramHandler.StopVideo();
    }
    public void PauseExperience()
    {
        hologramHandler.PauseVideo();
    }
    public void ResumeVideo()
    {
        hologramHandler.ResumeVideo();
    }
    public void SetOutOfFocus()
    {
        viewingExperienceInFocus = false;
    }
    void OnApplicationFocus(bool isFocused)
    {
        if (viewingExperienceInFocus && !isFocused)
        {
            //btnBurger.GetComponent<Button>().onClick?.Invoke();
        }
        if (!isFocused && activatedForStreaming == false)
        {
            //print("FOCUS PAUSE CALLED");
            //PauseExperience();
            //hologramHandler.StopVideo();
        }
        if (isFocused && activatedForStreaming == false)
        {
            //yield return new WaitForSeconds(2);
            //print("FOCUS RESUME CALLED");
            hologramHandler.ForcePlay();
            //ResumeVideo();
        }
    }
}