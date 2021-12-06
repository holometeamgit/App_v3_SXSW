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


public class ARMsgScreenRecorder : MonoBehaviour {

    [SerializeField] Camera _camera;
    private IMediaRecorder recorder;
    private CameraInput cameraInput;
    AudioDevice audioDevice;

    private string _lastPathVideo;

    async void Start() {

        CallBacks.OnStartRecord += StartRecording;
        CallBacks.OnStopRecord += StopRecord;

        // Request mic permissions
        if (!await MediaDeviceQuery.RequestPermissions<AudioDevice>()) {
            HelperFunctions.DevLogError("User did not grant microphone permissions");
            return;
        }

        CallBacks.OnGetVideoRecordedFilePath += GetPathToFile;
    }

    private void OnDestroy() {
        CallBacks.OnStartRecord -= StartRecording;
        CallBacks.OnStopRecord -= StopRecord;
        CallBacks.OnGetVideoRecordedFilePath -= GetPathToFile;
        StopRecord();
    }

    public void StartRecording() {
        var clock = new RealtimeClock();
        int width;
        int heigh;
        AgoraSharedVideoConfig.GetResolution(screenWidth: Screen.width, screenHeigh: Screen.height, out width, out heigh);

        // Create a media device query for audio devices
        var deviceQuery = new MediaDeviceQuery(MediaDeviceCriteria.AudioDevice);
        // Get the device
        audioDevice = deviceQuery.current as AudioDevice;

        HelperFunctions.DevLog("vide record width " + width + " heigh " + heigh);

        // Create recorder
        recorder = new MP4Recorder(width, heigh, AgoraSharedVideoConfig.FrameRate, audioDevice.sampleRate, audioDevice.channelCount);
        // Stream media samples
        cameraInput = new CameraInput(recorder, clock, _camera);
        audioDevice.StartRunning((sampleBuffer, timestamp) => recorder.CommitSamples(sampleBuffer, clock.timestamp));
    }

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
