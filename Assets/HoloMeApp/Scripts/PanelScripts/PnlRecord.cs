using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using NatSuite.Recorders;
using NatSuite.Recorders.Clocks;
using NatSuite.Recorders.Inputs;
using System.Collections;
using TMPro;
using UnityEngine.Events;
using System.IO;
using Beem.Utility;
using Beem.Permissions;
using System.Threading.Tasks;
using System.Threading;

public class PnlRecord : MonoBehaviour {
    [SerializeField]
    Camera canvasCamera;

    [SerializeField]
    Camera arCamera;

    [SerializeField]
    Image imgRecordFill;

    [SerializeField]
    Image imgFillBackground;

    [SerializeField]
    Button btnToggleMode;

    [SerializeField]
    TextMeshProUGUI txtVideo;

    [SerializeField]
    TextMeshProUGUI txtPhoto;

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
    private IMediaRecorder videoRecorder;
    private IClock recordingClock;
    private CameraInput cameraInput;
    private AudioInput audioInput;
    private Coroutine currentCoroutine;

    private PermissionController _permissionController;
    private PermissionController permissionController {
        get {

            if (_permissionController == null) {
                _permissionController = FindObjectOfType<PermissionController>();
            }

            return _permissionController;
        }
    }

    int videoWidth;
    int videoHeight;

    string lastRecordingPath;

    enum Mode { Video, Photo };
    Mode mode;
    Mode ChangeMode {
        set {
            mode = value;
            switch (mode) {
                case Mode.Video:
                    rtButtonContainer.DOAnchorPosX(videoButtonContainerPosition.x, .25f);
                    txtPhoto.color = HelperFunctions.GetColor(176, 176, 176);
                    txtVideo.color = Color.white;
                    imgRecordFill.gameObject.SetActive(true);
                    break;
                case Mode.Photo:
                    rtButtonContainer.DOAnchorPosX(videoButtonContainerPosition.x - rtButtonContainer.rect.width / 2, .25f);
                    txtPhoto.color = Color.white;
                    txtVideo.color = HelperFunctions.GetColor(176, 176, 176);
                    imgRecordFill.gameObject.SetActive(false);
                    break;
            }
        }
    }

    void Start() {
        CorrectResolutionAspect();
        ChangeMode = Mode.Video;
        btnToggleMode.onClick.AddListener(() => ChangeMode = mode == Mode.Video ? Mode.Photo : Mode.Video);
        videoButtonContainerPosition = rtButtonContainer.anchoredPosition;
        canvasGroup.alpha = 0;

        uiThumbnailsController.OnPlayFromUser += user => txtWaterMarkText.text = "@" + user; //Gameobject must be active in the editor for this to work correctly
    }

    private void OnEnable() {
        canvasGroup?.DOFade(1, .5f);
    }

    private void OnDisable() {
        canvasGroup.alpha = 0;
    }

    private void CorrectResolutionAspect() {
        videoWidth = MakeEven(Screen.width / 2);
        videoHeight = MakeEven(Screen.height / 2);
    }

    public int MakeEven(int value) {
        return value % 2 == 0 ? value : value - 1;
    }

    /// <summary>
    /// start recording
    /// </summary>
    public void StartRecording() {
        if (!permissionController.CheckMicAccess()) {
            recordMicrophone = false;
        }
        recordLengthFailed = false;
        recordingClock = new RealtimeClock();
        videoRecorder = new MP4Recorder(
            videoWidth,
            videoHeight,
            25,
            recordMicrophone ? AudioSettings.outputSampleRate : 0,
            recordMicrophone ? (int)AudioSettings.speakerMode : 0
        );



        cameraInput = new CameraInput(videoRecorder, recordingClock, arCamera, canvasCamera);
        if (recordMicrophone) {
            audioInput = new AudioInput(videoRecorder, recordingClock, videoSource);
        }

        btnToggleMode.interactable = false;
        Recording = true;
        OnRecordStarted?.Invoke();
        watermarkCanvasObject.SetActive(true);
    }

    public void RecordLengthFail() {
        OnRecordStopped?.Invoke();
        MakeScreenshot();
        recordLengthFailed = true;
    }

    public void StopRecording() {
        //CancelInvoke("Countdown");

        if (recordMicrophone) {
            audioInput.Dispose();
        }
        cameraInput.Dispose();

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        OnRecordComplete().ContinueWith((taskWebRequestData) => {

            imgRecordFill.fillAmount = 0;
            btnToggleMode.interactable = true;
            Recording = false;

            if (!recordLengthFailed)
                watermarkCanvasObject.SetActive(false);

        }, taskScheduler);

    }

    private async Task OnRecordComplete() {
        string outputPath = await videoRecorder.FinishWriting();
        if (recordLengthFailed) {
            File.Delete(outputPath);
            OnRecordStopped?.Invoke();
            MakeScreenshot();
        } else {
            lastRecordingPath = outputPath;
            OnRecordStopped?.Invoke();
            PostRecordARConstructor.OnActivatedVideo?.Invoke(outputPath);
        }
    }

    private void MakeScreenshot() {
        if (currentCoroutine == null) {
            //print("MAKING SCREENSHOT");
            OnSnapshotStarted?.Invoke();
            currentCoroutine = StartCoroutine(ScreenShotAsync());
        }
    }

    private IEnumerator ScreenShotAsync() {
        canvasGroup.alpha = 0;
        watermarkCanvasObject.SetActive(true);
        //print("ENABLED WATERMARK");
        HideUI.onActivate(false);
        yield return new WaitForEndOfFrame();

        Texture2D screenShot = ScreenCapture.CaptureScreenshotAsTexture(1);

        yield return new WaitForEndOfFrame();
        HideUI.onActivate(true);
        PostRecordARConstructor.OnActivatedScreenShot?.Invoke(Sprite.Create(screenShot, new Rect(0, 0, Screen.width, Screen.height), new Vector2(0.5f, 0.5f)), screenShot, lastRecordingPath);
        //pnlVideoExperience.PauseExperience();
        OnSnapshotEnded?.Invoke();
        canvasGroup.alpha = 1;
        currentCoroutine = null;
        watermarkCanvasObject.SetActive(false);
    }
}
