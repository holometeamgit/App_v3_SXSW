using UnityEngine;
using System.Collections;
using NatCorder;
using NatCorder.Clocks;
using NatCorder.Inputs;
using UnityEngine.Video;
using UnityEngine.UI;
using NatSuite.Devices;
using Beem.ARMsg;
using System.Threading.Tasks;
using Zenject;

/// <summary>
/// ARMsgScreenRecorder. Class for recording screen 
/// </summary>
public class ARMsgScreenRecorder : MonoBehaviour {

    [SerializeField]
    private Camera _camera;
    private IMediaRecorder recorder;
    private CameraInput cameraInput;
    private AudioDevice audioDevice;

    private string _lastPathVideo;
    private const int MAX_HEIGHT = 720;
    private const int MAX_HEIGHT_FOR_BUSINESS = 1920;
    private const int BITRATE = 4000000;

    private Coroutine _startingRecordingCoroutine;
    BusinessProfileManager _businessProfileManager;

    /// <summary>
    /// Start recording screen
    /// </summary>
    public void StartRecording() {
        if(_startingRecordingCoroutine != null) {
            StopCoroutine(_startingRecordingCoroutine);
        }
        _startingRecordingCoroutine = StartCoroutine(StartingRecording());
    }

    /// <summary>
    /// stop recording screeen
    /// </summary>
    public void StopRecord() {
        if (_startingRecordingCoroutine != null) {
            StopCoroutine(_startingRecordingCoroutine);
        }

        audioDevice?.StopRunning();
        cameraInput?.Dispose();
        recorder?.Dispose();
    }

    [Inject]
    public void Constructor(BusinessProfileManager businessProfileManager) {
        _businessProfileManager = businessProfileManager;
    }

    private async void Start() {

        CallBacks.OnStartRecord += StartRecording;
        CallBacks.OnStopRecord += StopRecord;
        CallBacks.OnGetVideoRecordedFilePath += GetPathToFile;
    }

    private void OnRecordComplete(string path) {
        Application.targetFrameRate = ApplicationSettingsHandler.TARGET_FRAAME_RATE;
        ApplicationSettingsHandler.Instance.ToggleSleepTimeout(false);
        // Playback recording
        HelperFunctions.DevLog($"Saved recording to: {path}");

        _lastPathVideo = path;

        CallBacks.OnVideoReadyPlay?.Invoke();
    }

    private string GetPathToFile() {
        return _lastPathVideo;
    }

    private void OnDestroy() {
        CallBacks.OnStartRecord -= StartRecording;
        CallBacks.OnStopRecord -= StopRecord;
        CallBacks.OnGetVideoRecordedFilePath -= GetPathToFile;
        StopRecord();
    }

    private IEnumerator StartingRecording() {
        var clock = new RealtimeClock();
        int width;
        int heigh;
        AgoraSharedVideoConfig.GetResolution(screenWidth: Screen.width, screenHeigh: Screen.height,
            out width, out heigh,
            maxHeigh: _businessProfileManager.IsBusinessProfile() ? MAX_HEIGHT_FOR_BUSINESS : MAX_HEIGHT);
        Application.targetFrameRate = AgoraSharedVideoConfig.FrameRate;
        ApplicationSettingsHandler.Instance.ToggleSleepTimeout(true);
        yield return null;
        // Create a media device query for audio devices
        var deviceQuery = new MediaDeviceQuery(MediaDeviceCriteria.AudioDevice);
        // Get the device
        audioDevice = deviceQuery.current as AudioDevice;

        // Create recorder
        recorder = new MP4Recorder(width, heigh,
            framerate: AgoraSharedVideoConfig.FrameRate,
            sampleRate: audioDevice.sampleRate, channelCount: audioDevice.channelCount,
            recordingCallback: OnRecordComplete,
            bitrate: BITRATE);

        // Stream media samples
        cameraInput = new CameraInput(recorder, clock, _camera);
        audioDevice.StartRunning((sampleBuffer, timestamp) => recorder.CommitSamples(sampleBuffer, clock.Timestamp));
    }

}
