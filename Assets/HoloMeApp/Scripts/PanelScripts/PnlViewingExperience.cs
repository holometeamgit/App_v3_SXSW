using UnityEngine.XR.ARFoundation;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class PnlViewingExperience : MonoBehaviour
{
    [SerializeField]
    GameObject scanAnimationItems;

    [SerializeField]
    GameObject btnBurger;

    [SerializeField]
    CanvasGroup canvasGroup;

    [SerializeField]
    PnlCameraAccess pnlCameraAccess;

    [SerializeField]
    RectTransform scanMessageRT;

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
    FocusSquare focusSquare;

    [SerializeField]
    PnlRecord pnlRecord;

    Coroutine scanAnimationRoutine;

    bool activatedForStreaming;
    bool viewingExperienceInFocus;
    bool tutorialDisplayed;

    float messageAnimationSpeed = 0.25f;
    float messageTime = 10;
    float animationSpeed = 0.25f;

    private enum TutorialState { MessageScan, MessageTapToPlace, WaitingForTap, TutorialComplete };
    TutorialState tutorialState = TutorialState.MessageScan;

    void OnEnable()
    {
        scanAnimationItems.SetActive(false);
        if (permissionGranter.HasCameraAccess && !tutorialDisplayed)
        {
            RunTutorial();
            tutorialDisplayed = true;
        }
        else
        {
            pnlCameraAccess.OnAccessGranted.AddListener(RunTutorial);
            pnlCameraAccess.gameObject.SetActive(true);
        }
    }

    public void RunTutorial()
    {
        if (!gameObject.activeSelf)
            return;

        if (tutorialState == TutorialState.TutorialComplete)
            return;

        if (scanAnimationRoutine != null)
            StopCoroutine(scanAnimationRoutine); //Stop old routine is reactivating

        scanAnimationRoutine = StartCoroutine(StartScanAnimationLoop(messageTime));
        ShowMessage("Scan the floor to start");
        tutorialState = TutorialState.MessageTapToPlace;
        arPlaneManager.enabled = true;
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
            ShowMessage("Tap screen to place", messageAnimationSpeed);
            tutorialState = TutorialState.WaitingForTap;
        }
    }

    public void OnPlaced()
    {
        if (tutorialState == TutorialState.WaitingForTap)
        {
            HideScanMessage();
            StartCoroutine(DelayStartRecordPanel(messageAnimationSpeed, activatedForStreaming));
            tutorialState = TutorialState.TutorialComplete;
        }
    }

    IEnumerator DelayStartRecordPanel(float delay, bool streamPanel)
    {
        yield return new WaitForSeconds(delay);
        pnlRecord.EnableRecordPanel(streamPanel);
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

    public void ActivateForPreRecorded(string code, VideoJsonData videoJsonData)
    {
        //print($"PLAY CALLED - " + code);
        activatedForStreaming = false;
        blurController.RemoveBlur();
        canvasGroup.alpha = 0;
        gameObject.SetActive(true);
        btnBurger.SetActive(true);
        logoCanvas.ActivateIfLogoAvailable(videoJsonData);
        hologramHandler.InitSession();
        hologramHandler.PlayIfPlaced(code);
        hologramHandler.TogglePreRecordedVideoRenderer(true);
        focusSquare.StartScanning = true;
        FadeInCanvas();

        if (tutorialState == TutorialState.TutorialComplete) //Re-enable record settings if tutorial was complete when coming back to viewing
        {
            HideScanMessage();
            StartCoroutine(DelayStartRecordPanel(messageAnimationSpeed, false));
        }

        viewingExperienceInFocus = true;
    }

    public void ActivateForStreaming()
    {
        activatedForStreaming = true;
        blurController.RemoveBlur();
        canvasGroup.alpha = 0;
        gameObject.SetActive(true);
        btnBurger.SetActive(false); //Close button not required on this page
        hologramHandler.InitSession();
        hologramHandler.TogglePreRecordedVideoRenderer(false);
        focusSquare.StartScanning = true;
        FadeInCanvas();
        StopExperience();

        if (tutorialState == TutorialState.TutorialComplete) //Re-enable record settings if tutorial was complete when coming back to viewing
        {
            HideScanMessage();
            StartCoroutine(DelayStartRecordPanel(messageAnimationSpeed, true));
        }

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
    }
}
