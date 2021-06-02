using System.IO;
using Beem.Record.SnapShot;
using Beem.Video;
using NatCorder;
using NatCorder.Clocks;
using NatCorder.Inputs;
using UnityEngine;

namespace Beem.Record.Video {
    /// <summary>
    /// Video Record Controller
    /// </summary>
    public class VideoRecordController : MonoBehaviour {

        [Header("Source for Audio")]
        [SerializeField]
        private AudioSource _audioSource;

        private bool recordLengthFailed = false;
        private bool recordMicrophone = true;

        private MP4Recorder videoRecorder;
        private IClock recordingClock;
        private CameraInput cameraInput;
        private AudioInput audioInput;
        //private PermissionGranter permissionGranter;
        private Camera[] cameras;

        private int videoWidth;
        private int videoHeight;

        private string lastRecordingPath;

        private void Awake() {
            //permissionGranter = FindObjectOfType<PermissionGranter>();
            cameras = FindObjectsOfType<Camera>();
        }

        private void Start() {
            CorrectResolutionAspect();
        }

        private void OnEnable() {
            VideoRecordCallbacks.onRecordStarted += OnRecordStart;
            VideoRecordCallbacks.onRecordStoped += OnRecordStop;
            VideoRecordCallbacks.onRecordFailed += OnRecordFail;
            VideoRecordCallbacks.onSetAudioSource += OnSetAudioSource;
        }

        private void OnDisable() {
            VideoRecordCallbacks.onRecordStarted -= OnRecordStart;
            VideoRecordCallbacks.onRecordStoped -= OnRecordStop;
            VideoRecordCallbacks.onRecordFailed -= OnRecordFail;
            VideoRecordCallbacks.onSetAudioSource -= OnSetAudioSource;
        }

        private void OnSetAudioSource(AudioSource audioSource) {
            _audioSource = audioSource;
        }

        private void CorrectResolutionAspect() {
            videoWidth = MakeEven(Screen.width / 2);
            videoHeight = MakeEven(Screen.height / 2);
        }

        public int MakeEven(int value) {
            return value % 2 == 0 ? value : value - 1;
        }

        private void OnRecordStart() {
            /*if (!permissionGranter.MicAccessAvailable) {
                recordMicrophone = false;
            }*/

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
                audioInput = new AudioInput(videoRecorder, recordingClock, _audioSource);
            }
        }

        private void OnRecordFail() {
            recordLengthFailed = true;
            OnRecordStop();
        }

        private void OnRecordStop() {
            if (recordMicrophone) {
                audioInput.Dispose();
            }

            cameraInput.Dispose();
            videoRecorder.Dispose();
        }

        private void OnRecordComplete(string outputPath) {
            if (recordLengthFailed) {
                File.Delete(outputPath);
                SnapShotCallBacks.onSnapshotStarted?.Invoke();
            } else {
                lastRecordingPath = outputPath;
                VideoRecordCallbacks.onRecordFinished?.Invoke(lastRecordingPath);
                recordLengthFailed = false;
            }
            VideoPlayerCallBacks.onPause?.Invoke();
        }

    }
}
