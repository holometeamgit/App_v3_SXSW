using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using NatCorder;
using NatCorder.Clocks;
using NatCorder.Inputs;
using NatShare;
using System.Collections;
using TMPro;
using UnityEngine.Events;

public class PnlRecord : MonoBehaviour
{
    [SerializeField]
    GameObject watermarkCanvasObject;

    [SerializeField]
    Camera canvasCamera;

    [SerializeField]
    Camera arCamera;

    [SerializeField]
    Image imgRecordFill;

    [SerializeField]
    Image imgFillBackground;

    [SerializeField]
    Sprite spriteTakeSnapshot;

    [SerializeField]
    Sprite spriteStop;

    [SerializeField]
    Sprite spriteRecord;

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
    UnityEvent OnRecordStarted;
    [SerializeField]
    UnityEvent OnRecordStopped;
    [SerializeField]
    UnityEvent OnSnapshotStarted;
    [SerializeField]
    UnityEvent OnSnapshotEnded;

    public bool Recording { get; set; }

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
    //float startRecordTime;
    //int recordTime;

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
    }

    private void OnEnable()
    {
        canvasGroup.DOFade(1, .5f);
    }

    private void OnDisable()
    {
        canvasGroup.alpha = 0;
    }

    private void CorrectResolutionAspect()
    {
        videoWidth = Screen.width / 2;
        videoHeight = Screen.height / 2;
        //videoWidth = 720;
        //videoHeight = (int)((float)videoWidth * ratio);
        //float ratio = (float)Screen.height / (float)Screen.width;
        //print("RES = " + (int)((float)videoWidth * ratio));
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
        OnRecordStopped?.Invoke();
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
        lastRecordingPath = outputPath;
        pnlPostRecord.ActivatePostVideo(outputPath);
    }

    void MakeScreenshot()
    {
        if (currentCoroutine == null)
        {
            OnSnapshotStarted?.Invoke();
            currentCoroutine = StartCoroutine(ScreenShotAsync());
        }
    }

    private IEnumerator ScreenShotAsync()
    {
        canvasGroup.alpha = 0;
        yield return new WaitForEndOfFrame();

        Texture2D screenShot = ScreenCapture.CaptureScreenshotAsTexture(1);

        yield return new WaitForEndOfFrame();

        pnlPostRecord.ActivatePostScreenshot(Sprite.Create(screenShot, new Rect(0, 0, Screen.width, Screen.height), new Vector2(0.5f, 0.5f)), screenShot, lastRecordingPath);
        //pnlVideoExperience.PauseExperience();
        OnSnapshotEnded?.Invoke();
        canvasGroup.alpha = 1;
        currentCoroutine = null;
    }
}
