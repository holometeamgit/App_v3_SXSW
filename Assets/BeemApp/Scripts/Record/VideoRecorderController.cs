using System.IO;
using NatCorder;
using NatCorder.Clocks;
using NatCorder.Inputs;
using UnityEngine;

public class VideoRecorderController : MonoBehaviour {

    [Header("Source for Audio")]
    [SerializeField]
    private AudioSource audioSource;

    private bool recordLengthFailed;
    private bool recordMicrophone = true;

    private MP4Recorder videoRecorder;
    private IClock recordingClock;
    private CameraInput cameraInput;
    private AudioInput audioInput;
    private PermissionGranter permissionGranter;
    private Camera[] cameras;

    private int videoWidth;
    private int videoHeight;

    private string lastRecordingPath;

    private void Awake() {
        permissionGranter = FindObjectOfType<PermissionGranter>();
        cameras = FindObjectsOfType<Camera>();
    }

    private void Start() {
        CorrectResolutionAspect();
    }

    private void CorrectResolutionAspect() {
        videoWidth = MakeEven(Screen.width / 2);
        videoHeight = MakeEven(Screen.height / 2);
    }

    public int MakeEven(int value) {
        return value % 2 == 0 ? value : value - 1;
    }

    private void StartRecording() {
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

        VideoRecorderCallbacks.onStartRecording?.Invoke();
    }

    public void StopRecording() {
        if (recordMicrophone) {
            audioInput.Dispose();
        }

        cameraInput.Dispose();
        videoRecorder.Dispose();
    }

    private void OnRecordComplete(string outputPath) {
        if (recordLengthFailed) {
            File.Delete(outputPath);
            VideoRecorderCallbacks.onStopRecording?.Invoke();
            SnapShotCallBacks.onSnapshotStarted?.Invoke();
        } else {
            lastRecordingPath = outputPath;
        }
    }

}
