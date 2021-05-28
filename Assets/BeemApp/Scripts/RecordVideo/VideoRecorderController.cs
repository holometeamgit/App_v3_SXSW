using System.Collections;
using System.IO;
using NatCorder;
using NatCorder.Clocks;
using NatCorder.Inputs;
using UnityEngine;
using UnityEngine.Events;

public class VideoRecorderController : MonoBehaviour {

    [SerializeField]
    private AudioSource audioSource;

    private bool recording { get; set; }
    private bool recordLengthFailed;
    private bool recordMicrophone = true;

    private MP4Recorder videoRecorder;
    private IClock recordingClock;
    private CameraInput cameraInput;
    private AudioInput audioInput;
    private Coroutine currentCoroutine;
    private PermissionGranter permissionGranter;
    private Camera[] cameras;
    private int videoWidth;
    private int videoHeight;

    private string lastRecordingPath;

    enum Mode { Video, Photo };
    private Mode mode;
    private Mode ChangeMode {
        set {
            mode = value;
        }
    }

    private void Awake() {
        permissionGranter = FindObjectOfType<PermissionGranter>();
        cameras = FindObjectsOfType<Camera>();
    }

    private void Start() {
        CorrectResolutionAspect();
        ChangeMode = Mode.Video;
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

        cameraInput = new CameraInput(videoRecorder, recordingClock, cameras);
        if (recordMicrophone) {
            audioInput = new AudioInput(videoRecorder, recordingClock, audioSource);
        }

        recording = true;

        VideoRecorderCallbacks.onStartRecording?.Invoke();
    }

    public void RecordLengthFail() {
        VideoRecorderCallbacks.onStopRecording?.Invoke();
        MakeScreenshot();
        recordLengthFailed = true;
    }

    public void StopRecording() {
        if (recordMicrophone) {
            audioInput.Dispose();
        }

        cameraInput.Dispose();
        videoRecorder.Dispose();
        recording = false;
    }

    private void OnRecordComplete(string outputPath) {
        if (recordLengthFailed) {
            File.Delete(outputPath);
            MakeScreenshot();
        } else {
            lastRecordingPath = outputPath;
        }
        VideoRecorderCallbacks.onStopRecording?.Invoke();
    }

    private void MakeScreenshot() {
        if (currentCoroutine == null) {
            VideoRecorderCallbacks.onSnapshotStarted?.Invoke();
            currentCoroutine = StartCoroutine(ScreenShotAsync());
        }
    }

    private IEnumerator ScreenShotAsync() {

        yield return new WaitForEndOfFrame();
        Texture2D screenShot = ScreenCapture.CaptureScreenshotAsTexture(1);
        yield return new WaitForEndOfFrame();
        Sprite spr = Sprite.Create(screenShot, new Rect(0, 0, Screen.width, Screen.height), Vector2.one * 0.5f);
        VideoRecorderCallbacks.onSnapshotEnded?.Invoke();
        currentCoroutine = null;
    }
}
