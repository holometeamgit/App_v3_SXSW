using UnityEngine;
using System.Collections;
using NatSuite.Recorders;
using NatSuite.Recorders.Clocks;
using NatSuite.Recorders.Inputs;
using UnityEngine.Video;
using UnityEngine.UI;
using NatSuite.Devices;
using Beem.ARMsg;
using System.Threading.Tasks;

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
    private const int MAX_HEIGH = 720;
    private const int BITRATE = 4000000;

    private async void Start() {

        CallBacks.OnStartRecord += StartRecording;
        CallBacks.OnStopRecord += StopRecord;
        CallBacks.OnGetVideoRecordedFilePath += GetPathToFile;
    }

    private void OnDestroy() {
        CallBacks.OnStartRecord -= StartRecording;
        CallBacks.OnStopRecord -= StopRecord;
        CallBacks.OnGetVideoRecordedFilePath -= GetPathToFile;
        StopRecord();
    }

    /// <summary>
    /// Start recording screen
    /// </summary>
    public void StartRecording() {
        var clock = new RealtimeClock();
        int width;
        int heigh;
        AgoraSharedVideoConfig.GetResolution(screenWidth: Screen.width, screenHeigh: Screen.height, out width, out heigh, maxHeigh: MAX_HEIGH);

        // Create a media device query for audio devices
        var deviceQuery = new MediaDeviceQuery(MediaDeviceCriteria.AudioDevice);
        // Get the device
        audioDevice = deviceQuery.current as AudioDevice;

        HelperFunctions.DevLog("vide record width " + width + " heigh " + heigh);

        // Create recorder
        recorder = new MP4Recorder(width, heigh, frameRate: AgoraSharedVideoConfig.FrameRate, audioDevice.sampleRate, audioDevice.channelCount, bitrate: BITRATE);
        // Stream media samples
        cameraInput = new CameraInput(recorder, clock, _camera);
        audioDevice.StartRunning((sampleBuffer, timestamp) => recorder.CommitSamples(sampleBuffer, clock.timestamp));
    }

    /// <summary>
    /// stop recording screeen
    /// </summary>
    public void StopRecord() {
        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        StopRecordingAsync().ContinueWith((task) => {
            CallBacks.OnVideoReadyPlay?.Invoke();
        }
        , taskScheduler);
    }

    private async Task StopRecordingAsync() {
        audioDevice.StopRunning();
        cameraInput.Dispose();

        var path = await recorder.FinishWriting();

        // Playback recording
        HelperFunctions.DevLog($"Saved recording to: {path}");

        _lastPathVideo = path;
    }

    private string GetPathToFile() {
        return _lastPathVideo;
    }


}
