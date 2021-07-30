using System.IO;
using Beem.Extenject.Hologram;
using Beem.Extenject.Record.SnapShot;
using Beem.Extenject.Record.Video;
using Beem.Extenject.UI;
using Beem.Extenject.Video;
using NatCorder;
using NatCorder.Clocks;
using NatCorder.Inputs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using Zenject;

namespace Beem.Extenject.Record {

    /// <summary>
    /// Video Record Controller
    /// </summary>
    public class VideoRecordController : IInitializable, ILateDisposable {

        private WindowSignal _windowSignals;
        private WindowController _windowController;
        private SnapShotController _snapShotController;

        private SignalBus _signalBus;

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

        public VideoRecordController(WindowSignal windowSignal, Camera[] cameras) {
            _windowSignals = windowSignal;
            _cameras = cameras;
            CorrectResolutionAspect();
        }

        [Inject]
        public void Construct(SignalBus signalBus, WindowController windowController, SnapShotController snapShotController) {
            _windowController = windowController;
            _snapShotController = snapShotController;
            _signalBus = signalBus;
        }

        public void Initialize() {
            HologramCallbacks.onCreateHologram += OnSetVideo;
        }

        public void LateDispose() {
            HologramCallbacks.onCreateHologram -= OnSetVideo;
        }

        public void OnSetVideo(GameObject audio) {
            _audioSource = audio.GetComponentInChildren<AudioSource>();
        }

        private void CorrectResolutionAspect() {
            _videoWidth = MakeEven(Screen.width / 2);
            _videoHeight = MakeEven(Screen.height / 2);
        }

        public int MakeEven(int value) {
            return value % 2 == 0 ? value : value - 1;
        }

        /// <summary>
        /// Record Start
        /// </summary>
        public void OnRecordStart() {
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

        /// <summary>
        /// Record Fail
        /// </summary>
        public void OnRecordFail() {
            _recordLengthFailed = true;
            OnRecordStop();
        }

        /// <summary>
        /// Record Stop
        /// </summary>
        public void OnRecordStop() {
            if (_recordMicrophone) {
                _audioInput.Dispose();
            }

            _cameraInput.Dispose();
            _videoRecorder.Dispose();
        }

        private void OnRecordComplete(string outputPath) {
            if (_recordLengthFailed) {
                File.Delete(outputPath);
                _snapShotController.CreateSnapShotAsync();
            } else {
                _lastRecordingPath = outputPath;
                _windowController.OnCalledSignal(_windowSignals, _lastRecordingPath);
                _recordLengthFailed = false;
            }
            _signalBus.Fire(new RecordSignal());
        }
    }
}
