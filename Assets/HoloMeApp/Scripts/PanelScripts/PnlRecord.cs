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
using Beem.Utility;
using Beem.Permissions;
using System.Threading.Tasks;
using System.Threading;
using Beem.Video;

public class PnlRecord : MonoBehaviour {

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
    GameObject watermarkCanvasObject;
    [SerializeField]
    Text txtWaterMarkText;

    [Space]

    [SerializeField]
    private UIThumbnailsController _uiThumbnailsController;

    [SerializeField]
    private HologramHandler _hologramHandler;

    private Camera[] _cameras;

    private bool recordMicrophone = true;

    public bool Recording { get; set; }

    private IMediaRecorder videoRecorder;
    private IClock recordingClock;
    private CameraInput cameraInput;
    private AudioInput audioInput;
    private Coroutine currentCoroutine;

    private bool recordLengthFailed;

    private PermissionController _permissionController;
    private PermissionController permissionController {
        get {

            if (_permissionController == null) {
                _permissionController = FindObjectOfType<PermissionController>();
            }

            return _permissionController;
        }
    }

    private VideoPlayerController _videoPlayerController;
    private VideoPlayerController videoPlayerController {
        get {

            if (_videoPlayerController == null) {
                _videoPlayerController = FindObjectOfType<VideoPlayerController>();
            }

            return _videoPlayerController;
        }
    }

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
        ChangeMode = Mode.Video;
        btnToggleMode.onClick.AddListener(() => ChangeMode = mode == Mode.Video ? Mode.Photo : Mode.Video);
        videoButtonContainerPosition = rtButtonContainer.anchoredPosition;
        canvasGroup.alpha = 0;

        _uiThumbnailsController.OnPlayFromUser += user => txtWaterMarkText.text = "@" + user; //Gameobject must be active in the editor for this to work correctly
    }

    private void OnEnable() {
        canvasGroup?.DOFade(1, .5f);
    }

    private void OnDisable() {
        canvasGroup.alpha = 0;
    }

    /// <summary>
    /// start recording
    /// </summary>
    public void StartRecording() {
        if (!permissionController.PermissionGranter.HasMicAccess) {
            recordMicrophone = false;
        }

        int videoWidth;
        int videoHeight;

        AgoraSharedVideoConfig.GetResolution(screenWidth: Screen.width, screenHeigh: Screen.height, out videoWidth, out videoHeight);

        _cameras = FindObjectsOfType<Camera>();
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



        cameraInput = new CameraInput(videoRecorder, recordingClock, _cameras);
        if (recordMicrophone) {
            audioInput = new AudioInput(videoRecorder, recordingClock, _hologramHandler.GetAudioSource());
        }

        btnToggleMode.interactable = false;
        Recording = true;
        watermarkCanvasObject.SetActive(true);
    }

    public void RecordLengthFail() {
        videoPlayerController?.OnPause();
        MakeScreenshot();
        recordLengthFailed = true;
    }

    public void StopRecording() {
        //CancelInvoke("Countdown");

        if (recordMicrophone) {
            audioInput.Dispose();
        }
        cameraInput.Dispose();
        videoPlayerController?.OnPause();
    }

    private void OnRecordComplete(string path) {
        if (recordLengthFailed) {
            File.Delete(path);
            MakeScreenshot();
        } else {
            lastRecordingPath = path;
            PostRecordARConstructor.OnActivatedVideo?.Invoke(path);
        }

        imgRecordFill.fillAmount = 0;
        btnToggleMode.interactable = true;
        Recording = false;

        if (!recordLengthFailed)
            watermarkCanvasObject.SetActive(false);
    }

    private void MakeScreenshot() {
        if (currentCoroutine == null) {
            currentCoroutine = StartCoroutine(ScreenShotAsync());
        }
    }

    private IEnumerator ScreenShotAsync() {
        canvasGroup.alpha = 0;
        watermarkCanvasObject.SetActive(true);
        HideUI.onActivate(false);
        yield return new WaitForEndOfFrame();

        Texture2D screenShot = ScreenCapture.CaptureScreenshotAsTexture(1);

        yield return new WaitForEndOfFrame();
        HideUI.onActivate(true);
        PostRecordARConstructor.OnActivatedScreenShot?.Invoke(Sprite.Create(screenShot, new Rect(0, 0, Screen.width, Screen.height), new Vector2(0.5f, 0.5f)), screenShot, lastRecordingPath);
        canvasGroup.alpha = 1;
        currentCoroutine = null;
        watermarkCanvasObject.SetActive(false);
    }
}
