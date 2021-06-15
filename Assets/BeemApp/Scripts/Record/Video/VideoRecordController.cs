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

        private AudioSource _audioSource;
        private MP4Recorder _videoRecorder;
        private IClock _recordingClock;
        private CameraInput _cameraInput;
        private AudioInput _audioInput;
        //private PermissionGranter permissionGranter;
        private Camera[] _cameras;

        private int _videoWidth;
        private int _videoHeight;

        private string _lastRecordingPath;

        private bool _recordLengthFailed = false;
        private bool _recordMicrophone = true;

        private void Awake() {
            //permissionGranter = FindObjectOfType<PermissionGranter>();
            _cameras = FindObjectsOfType<Camera>();
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
            _videoWidth = MakeEven(Screen.width / 2);
            _videoHeight = MakeEven(Screen.height / 2);
        }

        public int MakeEven(int value) {
            return value % 2 == 0 ? value : value - 1;
        }

        private void OnRecordStart() {
            /*if (!permissionGranter.MicAccessAvailable) {
                recordMicrophone = false;
            }*/

            _recordLengthFailed = false;
            _recordingClock = new RealtimeClock();
            _videoRecorder = new MP4Recorder(
                _videoWidth,
                _videoHeight,
                25,
                _recordMicrophone ? AudioSettings.outputSampleRate : 0,
                _recordMicrophone ? (int)AudioSettings.speakerMode : 0,
                OnRecordComplete
            );

            _cameraInput = new CameraInput(_videoRecorder, _recordingClock, _cameras);
            if (_recordMicrophone) {
                _audioInput = new AudioInput(_videoRecorder, _recordingClock, _audioSource);
            }
        }

        private void OnRecordFail() {
            _recordLengthFailed = true;
            OnRecordStop();
        }

        private void OnRecordStop() {
            if (_recordMicrophone) {
                _audioInput.Dispose();
            }

            _cameraInput.Dispose();
            _videoRecorder.Dispose();
        }

        private void OnRecordComplete(string outputPath) {
            if (_recordLengthFailed) {
                File.Delete(outputPath);
                SnapShotCallBacks.onSnapshotStarted?.Invoke();
            } else {
                _lastRecordingPath = outputPath;
                VideoRecordCallbacks.onPostRecord?.Invoke();
                VideoRecordCallbacks.onRecordFinished?.Invoke(_lastRecordingPath);
                _recordLengthFailed = false;
            }
            VideoPlayerCallBacks.onPause?.Invoke();
        }

    }
}
