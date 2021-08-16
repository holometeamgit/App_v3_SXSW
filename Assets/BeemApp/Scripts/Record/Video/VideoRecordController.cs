using System.IO;
using Beem.Extenject.Hologram;
using Beem.Extenject.UI;
using Beem.Video;
using NatCorder;
using NatCorder.Clocks;
using NatCorder.Inputs;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Record {

    /// <summary>
    /// Video Record Controller
    /// </summary>
    public class VideoRecordController : IInitializable, ILateDisposable {

        private WindowSignal _windowSignals;
        private WindowController _windowController;
        private SnapShotController _snapShotController;

        private AudioSource _audioSource;
        private IMediaRecorder _mediaRecorder;
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
        private SignalBus _signalBus;

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
            HologramCallbacks.onCreateHologram += OnCreateHologram;
            _signalBus.Subscribe<RecordStartSignal>(OnRecordStart);
            _signalBus.Subscribe<RecordEndSignal>(OnRecordEnd);
        }

        public void LateDispose() {
            HologramCallbacks.onCreateHologram -= OnCreateHologram;
            _signalBus.Unsubscribe<RecordStartSignal>(OnRecordStart);
            _signalBus.Unsubscribe<RecordEndSignal>(OnRecordEnd);
        }

        public void OnCreateHologram(GameObject hologram) {
            _audioSource = hologram.GetComponentInChildren<AudioSource>();
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
            _mediaRecorder = new MP4Recorder(
                _videoWidth,
                _videoHeight,
                25,
                _recordMicrophone ? AudioSettings.outputSampleRate : 0,
                _recordMicrophone ? (int)AudioSettings.speakerMode : 0,
                OnRecordComplete
            );

            _cameraInput = new CameraInput(_mediaRecorder, _recordingClock, _cameras);
            if (_recordMicrophone) {
                _audioInput = new AudioInput(_mediaRecorder, _recordingClock, _audioSource);
            }
        }

        private void OnRecordEnd(RecordEndSignal recordEndSignal) {
            if (recordEndSignal.Success) {
                OnRecordSuccess();
            } else {
                OnRecordFail();
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
        /// Record Success
        /// </summary>
        public void OnRecordSuccess() {
            _recordLengthFailed = false;
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
            _mediaRecorder.Dispose();
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
        }
    }
}
