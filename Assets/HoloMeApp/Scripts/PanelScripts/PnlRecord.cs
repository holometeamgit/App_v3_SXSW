using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using NatCorder;
using NatCorder.Clocks;
using NatCorder.Inputs;
using System.Collections;
using TMPro;
using UnityEngine.Events;
using System.IO;

public class PnlRecord : MonoBehaviour
{
    [SerializeField]
    Camera canvasCamera;

    [SerializeField]
    Camera arCamera;

    [SerializeField]
    Image imgRecordFill;

    [SerializeField]
    Image imgFillBackground;

    [SerializeField]
    Sprite spriteRecord;

    [SerializeField]
    Button btnShare;

    [SerializeField]
    Button btnToggleMode;

    //[SerializeField]
    //Button btnRecord;

    [SerializeField]
    TextMeshProUGUI txtVideo;

    [SerializeField]
    TextMeshProUGUI txtPhoto;

    [SerializeField]
    PnlPostRecord pnlPostRecord;

    [SerializeField]
    PermissionGranter permissionGranter;

    [SerializeField]
    RectTransform rtButtonContainer;

    [SerializeField]
    Vector2 videoButtonContainerPosition;

    [SerializeField]
    CanvasGroup canvasGroup;

    [SerializeField]
    RectTransform rectTeaser;

    [SerializeField]
    RectTransform rectNormal;

    [SerializeField]
    Button btnBuyTickets;
    [SerializeField]
    PurchaseManager purchaseManager;

    [SerializeField]
    UnityEvent OnRecordStarted;
    [SerializeField]
    UnityEvent OnRecordStopped;
    [SerializeField]
    UnityEvent OnSnapshotStarted;
    [SerializeField]
    UnityEvent OnSnapshotEnded;

    [SerializeField]
    UIThumbnailsController uiThumbnailsController;
    [SerializeField]
    GameObject watermarkCanvasObject;
    [SerializeField]
    Text txtWaterMarkText;

    public bool Recording { get; set; }
    private bool recordLengthFailed;

    [Header("Microphone")]
    public bool recordMicrophone = true;
    public AudioSource videoSource;
    //public AudioSource micSource;
    private MP4Recorder videoRecorder;
    private IClock recordingClock;
    private CameraInput cameraInput;
    private AudioInput audioInput;
    private Coroutine currentCoroutine;

    //const int RecordTimeLimit = 15;
    int videoWidth;
    int videoHeight;

    string lastRecordingPath;

    enum Mode { Video, Photo };
    Mode mode;
    Mode ChangeMode
    {
        set
        {
            mode = value;
            switch (mode)
            {
                case Mode.Video:
                    rtButtonContainer.DOAnchorPosX(videoButtonContainerPosition.x, .25f);
                    txtPhoto.color = HelperFunctions.GetColor(176, 176, 176);
                    txtVideo.color = Color.white;
                    //btnRecord.GetComponent<Image>().sprite = spriteRecord;
                    imgRecordFill.gameObject.SetActive(true);
                    break;
                case Mode.Photo:
                    rtButtonContainer.DOAnchorPosX(videoButtonContainerPosition.x - rtButtonContainer.rect.width / 2, .25f);
                    txtPhoto.color = Color.white;
                    txtVideo.color = HelperFunctions.GetColor(176, 176, 176);
                    //btnRecord.GetComponent<Image>().sprite = spriteTakeSnapshot;
                    imgRecordFill.gameObject.SetActive(false);
                    break;
            }
            //imgFillBackground.enabled = false;
        }
    }

    void Start()
    {
        CorrectResolutionAspect();
        ChangeMode = Mode.Video;
        btnToggleMode.onClick.AddListener(() => ChangeMode = mode == Mode.Video ? Mode.Photo : Mode.Video);
        //btnRecord.onClick.AddListener(() =>
        //{
        //    if (mode == Mode.Photo)
        //    {
        //        MakeScreenshot();
        //    }
        //    else
        //    {
        //        ToggleRecord();
        //    }
        //});
        //btnRecord.GetComponent<Image>().sprite = spriteRecord;
        videoButtonContainerPosition = rtButtonContainer.anchoredPosition;
        canvasGroup.alpha = 0;

        purchaseManager.OnPurchaseSuccessful += RefreshBuyBtnState;
        uiThumbnailsController.OnPlayFromUser += user => txtWaterMarkText.text = "@" + user; //Gameobject must be active in the editor for this to work correctly
        gameObject.SetActive(false);
    }

    public void EnableRecordPanel(bool isTeaser, bool openForStream = false)
    {
        //int buttonOffset = streamOffset ? 210 : 0;
        //imgFillBackground.rectTransform.offsetMax = new Vector2(imgFillBackground.rectTransform.offsetMax.x, buttonOffset);
        //imgFillBackground.rectTransform.offsetMin = new Vector2(imgFillBackground.rectTransform.offsetMin.x, buttonOffset);

        btnBuyTickets.gameObject.SetActive(isTeaser && !purchaseManager.IsBought());
        AssignRectTransform(imgFillBackground.rectTransform, isTeaser ? rectTeaser : rectNormal);
        btnShare.gameObject.SetActive(openForStream || isTeaser ? false : true);

        gameObject.SetActive(true);
        canvasGroup.DOFade(1, .5f);
    }

    private void RefreshBuyBtnState()
    {
        btnBuyTickets.gameObject.SetActive(purchaseManager.IsBought() ? false : btnBuyTickets.gameObject.activeSelf);
    }

    private void AssignRectTransform(RectTransform transformToAssign, RectTransform reference)
    {
        transformToAssign.anchoredPosition = reference.anchoredPosition;
        transformToAssign.anchorMax = reference.anchorMax;
        transformToAssign.anchorMin = reference.anchorMin;
    }

    private void OnDisable()
    {
        canvasGroup.alpha = 0;
    }

    private void CorrectResolutionAspect()
    {
        videoWidth = MakeEven(Screen.width / 2);
        videoHeight = MakeEven(Screen.height / 2);
        //videoWidth = Screen.width;
        //videoHeight = Screen.height;
        //print($"{videoWidth} x {videoHeight}");
        //videoWidth = 720;
        //videoHeight = (int)((float)videoWidth * ratio);
        //float ratio = (float)Screen.height / (float)Screen.width;
        //print("RES = " + (int)((float)videoWidth * ratio));
    }

    public int MakeEven(int value)
    {
        return value % 2 == 0 ? value : value - 1;
    }

    //public void ToggleRecord()
    //{
    //    if (Recording)
    //    {
    //        StopRecording();
    //    }
    //    else
    //    {
    //        StartRecording();
    //    }
    //}

    public void StartRecording()
    {
        //startRecordTime = Time.time;
        //recordTime = 0;

        //if (!permissionGranter.MicAccessAvailable && !permissionGranter.MicRequestComplete)
        //{
        //    permissionGranter.RequestMicAccess();
        //    return;
        //}

        if (!permissionGranter.MicAccessAvailable)
        {
            recordMicrophone = false;
        }
        recordLengthFailed = false;
        recordingClock = new RealtimeClock();
        videoRecorder = new MP4Recorder(
            videoWidth,
            videoHeight,
            25,
            recordMicrophone ? AudioSettings.outputSampleRate : 0,
            recordMicrophone ? (int)AudioSettings.speakerMode : 0,
            OnRecordComplete
        );

        cameraInput = new CameraInput(videoRecorder, recordingClock, arCamera, canvasCamera);
        if (recordMicrophone)
        {
            StartMicrophone();
            audioInput = new AudioInput(videoRecorder, recordingClock, videoSource);
        }

        //imgFillBackground.enabled = true;
        //btnRecord.GetComponent<Image>().sprite = spriteStop;
        btnToggleMode.interactable = false;
        //InvokeRepeating("Countdown", 0, 1);
        Recording = true;
        OnRecordStarted?.Invoke();
        watermarkCanvasObject.SetActive(true);
    }

    //private void Update()
    //{
    //    if (Recording)
    //    {
    //        float timeRecording = (Time.time - startRecordTime);
    //        imgRecordFill.fillAmount = timeRecording / RecordTimeLimit;
    //    }
    //}

    //private void Countdown()
    //{
    //    recordTime += 1;
    //    if (recordTime >= RecordTimeLimit)
    //    {
    //        StopRecording();
    //    }
    //}

    void StartMicrophone()
    {
#if !UNITY_WEBGL || UNITY_EDITOR // No `Microphone` API on WebGL :( 
        //// Create a microphone clip
        //micSource.clip = Microphone.Start(null, true, 15, 48000);
        //while (Microphone.GetPosition(null) <= 0) { print("Looping"); } //ADD MIC ACCESS CHECK HERE
        //// Play through audio source
        //micSource.timeSamples = Microphone.GetPosition(null);
        //micSource.loop = true;
        //micSource.Play();
#endif
    }

    public void RecordLengthFail()
    {
        OnRecordStopped?.Invoke();
        MakeScreenshot();
        recordLengthFailed = true;
    }

    public void StopRecording()
    {
        //CancelInvoke("Countdown");

        if (recordMicrophone)
        {
            StopMicrophone();
            audioInput.Dispose();
        }

        cameraInput.Dispose();
        videoRecorder.Dispose();
        //pnlVideoExperience.PauseExperience();
        imgRecordFill.fillAmount = 0;
        //btnRecord.GetComponent<Image>().sprite = spriteRecord;
        //imgFillBackground.enabled = false;
        btnToggleMode.interactable = true;
        Recording = false;
        //print("DISABLING HERE");

        if (!recordLengthFailed)
            watermarkCanvasObject.SetActive(false);
    }

    void StopMicrophone()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        //Microphone.End(null);
        //micSource.Stop();
#endif
    }

    void OnRecordComplete(string outputPath)
    {
        if (recordLengthFailed)
        {
            File.Delete(outputPath);
            OnRecordStopped?.Invoke();
            MakeScreenshot();
        }
        else
        {
            lastRecordingPath = outputPath;
            OnRecordStopped?.Invoke();
            pnlPostRecord.ActivatePostVideo(outputPath);
        }
    }

    void MakeScreenshot()
    {
        if (currentCoroutine == null)
        {
            //print("MAKING SCREENSHOT");
            OnSnapshotStarted?.Invoke();
            currentCoroutine = StartCoroutine(ScreenShotAsync());
        }
    }

    private IEnumerator ScreenShotAsync()
    {
        canvasGroup.alpha = 0;
        watermarkCanvasObject.SetActive(true);
        //print("ENABLED WATERMARK");

        yield return new WaitForEndOfFrame();

        Texture2D screenShot = ScreenCapture.CaptureScreenshotAsTexture(1);

        yield return new WaitForEndOfFrame();

        pnlPostRecord.ActivatePostScreenshot(Sprite.Create(screenShot, new Rect(0, 0, Screen.width, Screen.height), new Vector2(0.5f, 0.5f)), screenShot, lastRecordingPath);
        //pnlVideoExperience.PauseExperience();
        OnSnapshotEnded?.Invoke();
        canvasGroup.alpha = 1;
        currentCoroutine = null;
        watermarkCanvasObject.SetActive(false);
    }
}
