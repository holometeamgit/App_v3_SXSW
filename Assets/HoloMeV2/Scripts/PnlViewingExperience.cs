using UnityEngine.XR.ARFoundation;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Collections;
using UnityEngine.Events;

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

    //[SerializeField]
    //RectTransform messageRT;

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
    PnlRecord pnlRecord;

    [SerializeField]
    UnityEvent OnSurfaceDetected;

    //Coroutine scanAnimationRoutine;

    bool tutorialDisplayed;

    float messageAnimationSpeed = 0.25f;
    float messageTime = 6;
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

    private void RunTutorial()
    {
        //scanAnimationRoutine = StartCoroutine(StartScanAnimationLoop(messageTime));
        //ShowMessage("Scan the floor to start");
        ShowScanAnimation(animationSpeed);
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
                delay = delay * 1.5f;
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

        scanMessageRT.localScale = Vector3.zero;
        scanMessageRT.gameObject.SetActive(true);
        scanMessageRT.DOScale(Vector3.one, animationSpeed).SetDelay(0.5f);
    }

    private void HideScanAnimation(float animationSpeed)
    {
        scanAnimationItems.transform.DOScale(Vector3.zero, animationSpeed).OnComplete(() =>
        {
            scanAnimationItems.SetActive(false);
        });

        scanMessageRT.DOScale(Vector3.zero, animationSpeed).OnComplete(() =>
        {
            scanMessageRT.gameObject.SetActive(false);
        });
    }

    public void ShowTapToPlaceMessage()
    {
        //if (tutorialState == TutorialState.MessageTapToPlace)
        //{
        //    StopCoroutine(scanAnimationRoutine);
        //    HideScanAnimation(animationSpeed);
        //    HideMessage();
        //    ShowMessage("Tap screen to place", messageAnimationSpeed);
        //    tutorialState = TutorialState.WaitingForTap;
        //}
    }

    public void SkipTutorialStepOne()
    {
        OnSurfaceDetected?.Invoke();
    }

    public void SkipTutorialStepTwo()
    {
        //if (scanAnimationItems != null)
        //{
        //    StopCoroutine(scanAnimationRoutine);
        //}
        HideScanAnimation(animationSpeed);
        //HideMessage();
        StartCoroutine(DelayStartRecordPanel(messageAnimationSpeed));
        tutorialState = TutorialState.TutorialComplete;
    }

    public void OnPlaced()
    {
        if (tutorialState == TutorialState.WaitingForTap)
        {
            StartCoroutine(DelayStartRecordPanel(messageAnimationSpeed));
            tutorialState = TutorialState.TutorialComplete;
        }
    }

    IEnumerator DelayStartRecordPanel(float delay)
    {
        yield return new WaitForSeconds(delay);
        //HideMessage();
        pnlRecord.gameObject.SetActive(true);
    }

    //public void ShowMessage(string message, float delay = 0)
    //{
    //    messageRT.GetComponentInChildren<TextMeshProUGUI>().text = message;
    //    messageRT.DOAnchorPosY(messageRT.rect.height, messageAnimationSpeed).SetDelay(delay);
    //}

    //public void HideMessage()
    //{
    //    messageRT.DOAnchorPosY(0, messageAnimationSpeed);
    //}

    public void ActivateSelf(string code, VideoJsonData videoJsonData)
    {
        //print($"PLAY CALLED - " + code);
        blurController.RemoveBlur();
        canvasGroup.alpha = 0;
        gameObject.SetActive(true);
        btnBurger.SetActive(true);
        logoCanvas.ActivateIfLogoAvailable(videoJsonData);
        hologramHandler.InitSession(code);
        FadeInCanvas();

        if (tutorialState == TutorialState.TutorialComplete) //Re-enable record settings if tutorial was complete when coming back to viewing
        {
            StartCoroutine(DelayStartRecordPanel(messageAnimationSpeed));
        }
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
}
