using System.IO;
using Beem.Extenject.Hologram;
using Beem.Extenject.Permissions;
using NatCorder;
using NatCorder.Clocks;
using NatCorder.Inputs;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Record {

    /// <summary>
    /// Record System
    /// </summary>
    public class RecordSystem : IInitializable, ILateDisposable {

        private AudioSource _audioSource;
        private IMediaRecorder _mediaRecorder;
        private IClock _recordingClock;
        private CameraInput _cameraInput;
        private AudioInput _audioInput;
        private Camera[] _cameras;

        private int _videoWidth;
        private int _videoHeight;

        private string _lastRecordingPath;

        private bool _recordLengthFailed = false;
        private SignalBus _signalBus;

        public RecordSystem(Camera[] cameras) {
            _cameras = cameras;
            CorrectResolutionAspect();
        }

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        public void Initialize() {
            _signalBus.Subscribe<CreateHologramSignal>(OnCreateHologram);
            _signalBus.Subscribe<VideoRecordStartSignal>(OnRecordStart);
            _signalBus.Subscribe<VideoRecordEndSignal>(OnRecordEnd);
        }

        public void LateDispose() {
            _signalBus.Unsubscribe<CreateHologramSignal>(OnCreateHologram);
            _signalBus.Unsubscribe<VideoRecordStartSignal>(OnRecordStart);
            _signalBus.Unsubscribe<VideoRecordEndSignal>(OnRecordEnd);
        }

        public void OnCreateHologram(CreateHologramSignal createHologramSignal) {
            _audioSource = createHologramSignal.Hologram.GetComponentInChildren<AudioSource>();
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

            _recordLengthFailed = false;
            _recordingClock = new RealtimeClock();
            _mediaRecorder = new MP4Recorder(
                _videoWidth,
                _videoHeight,
                25,
                AudioSettings.outputSampleRate,
                (int)AudioSettings.speakerMode,
                OnRecordComplete
            );

            _cameraInput = new CameraInput(_mediaRecorder, _recordingClock, _cameras);
            _audioInput = new AudioInput(_mediaRecorder, _recordingClock, _audioSource);
        }

        private void OnRecordEnd(VideoRecordEndSignal recordEndSignal) {
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
            _audioInput.Dispose();
            _cameraInput.Dispose();
            _mediaRecorder.Dispose();
        }

        private void OnRecordComplete(string outputPath) {
            if (_recordLengthFailed) {
                File.Delete(outputPath);
                _signalBus.Fire(new SnapShotStartSignal());
            } else {
                _lastRecordingPath = outputPath;
                _signalBus.Fire(new VideoRecordFinishSignal(_lastRecordingPath));
                _recordLengthFailed = false;
            }
        }

    }
}
