using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Zenject;

namespace Beem.Extenject.Record {
    /// <summary>
    /// Record Button
    /// </summary>
    public class VideoRecordController : IInitializable, ILateDisposable {

        private RecordSystem _videoRecordController;
        private SignalBus _signalBus;
        private CancellationTokenSource cancelTokenSource;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        /// <summary>
        /// Record Async
        /// </summary>
        private async void RecordAsync(VideoRecordStartSignal recordStartSignal) {
            cancelTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancelTokenSource.Token;
            try {
                float timer = 0;
                float startTime = Time.time;
                VideoRecordProgressSignal recordProgressSignal = new VideoRecordProgressSignal();
                while (timer < recordStartSignal.RecordingTime.y && !cancellationToken.IsCancellationRequested) {
                    timer = Time.time - startTime;
                    recordProgressSignal.Progress = timer / recordStartSignal.RecordingTime.y;
                    _signalBus.Fire(recordProgressSignal);
                    await Task.Yield();
                }

                _signalBus.Fire(new VideoRecordEndSignal(timer >= recordStartSignal.RecordingTime.x));
            } finally {
                if (cancelTokenSource != null) {
                    cancelTokenSource.Dispose();
                    cancelTokenSource = null;
                }
            }
        }

        /// <summary>
        /// Cancel
        /// </summary>
        public void Cancel() {
            if (cancelTokenSource != null) {
                cancelTokenSource.Cancel();
                cancelTokenSource = null;
            }
        }

        public void Initialize() {
            _signalBus.Subscribe<VideoRecordStartSignal>(RecordAsync);
            _signalBus.Subscribe<VideoRecordStopSignal>(Cancel);
        }

        public void LateDispose() {
            _signalBus.Unsubscribe<VideoRecordStartSignal>(RecordAsync);
            _signalBus.Unsubscribe<VideoRecordStopSignal>(Cancel);
        }
    }
}