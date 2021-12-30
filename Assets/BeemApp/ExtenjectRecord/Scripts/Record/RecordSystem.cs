using System.IO;
using System.Threading.Tasks;
using Beem.Extenject.Hologram;
using Beem.Extenject.Permissions;
using Beem.Extenject.Video;
using NatCorder;
using NatSuite.Recorders;
using NatSuite.Recorders.Clocks;
using NatSuite.Recorders.Inputs;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Record {

    /// <summary>
    /// Record System
    /// </summary>
    public class RecordSystem : IInitializable, ILateDisposable {

        public AudioSource Audio {
            get {
                return _videoPlayerController.Player.GetTargetAudioSource(0);
            }
        }

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
        private VideoPlayerController _videoPlayerController;

        public RecordSystem(Camera[] cameras) {
            _cameras = cameras;
            CorrectResolutionAspect();
        }

        [Inject]
        public void Construct(SignalBus signalBus, VideoPlayerController videoPlayerController) {
            _videoPlayerController = videoPlayerController;
            _signalBus = signalBus;
        }

        public void Initialize() {
            _signalBus.Subscribe<VideoRecordStartSignal>(OnRecordStart);
            _signalBus.Subscribe<VideoRecordEndSignal>(OnRecordEnd);
        }

        public void LateDispose() {
            _signalBus.Unsubscribe<VideoRecordStartSignal>(OnRecordStart);
            _signalBus.Unsubscribe<VideoRecordEndSignal>(OnRecordEnd);
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
                (int)AudioSettings.speakerMode
            );

            _cameraInput = new CameraInput(_mediaRecorder, _recordingClock, _cameras);
            _audioInput = new AudioInput(_mediaRecorder, _recordingClock, Audio);
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
            OnRecordComplete();
        }

        private async void OnRecordComplete() {
            string outputPath = await _mediaRecorder.FinishWriting();
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
