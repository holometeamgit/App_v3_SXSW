﻿using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using NatCorder;
using NatCorder.Clocks;
using NatCorder.Inputs;
using System.Collections;
using TMPro;
using UnityEngine.Events;
using System.IO;

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
    Sprite spriteRecord;

    [SerializeField]
    Button btnToggleMode;

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
    private MP4Recorder videoRecorder;
    private IClock recordingClock;
    private CameraInput cameraInput;
    private AudioInput audioInput;
    private Coroutine currentCoroutine;

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

        purchaseManager.OnPurchaseSuccessful += RefreshBuyBtnState;
        uiThumbnailsController.OnPlayFromUser += user => txtWaterMarkText.text = "@" + user; //Gameobject must be active in the editor for this to work correctly
        gameObject.SetActive(false);
    }

    public void EnableRecordPanel(bool isTeaser, StreamJsonData.Data data, bool openForStream = false) {

        btnBuyTickets?.gameObject?.SetActive(isTeaser && !purchaseManager.IsBought());
        gameObject.SetActive(true);
        canvasGroup?.DOFade(1, .5f);
    }

    private void RefreshBuyBtnState() {
        btnBuyTickets.gameObject.SetActive(purchaseManager.IsBought() ? false : btnBuyTickets.gameObject.activeSelf);
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

    public void StartRecording() {
        if (!permissionGranter.MicAccessAvailable) {
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
        videoRecorder.Dispose();
        imgRecordFill.fillAmount = 0;
        btnToggleMode.interactable = true;
        Recording = false;

        if (!recordLengthFailed)
            watermarkCanvasObject.SetActive(false);
    }

    void OnRecordComplete(string outputPath) {
        if (recordLengthFailed) {
            File.Delete(outputPath);
            OnRecordStopped?.Invoke();
            MakeScreenshot();
        } else {
            lastRecordingPath = outputPath;
            OnRecordStopped?.Invoke();
            pnlPostRecord.ActivatePostVideo(outputPath);
        }
    }

    void MakeScreenshot() {
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
