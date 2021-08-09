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
    public class RecordController : IInitializable, ILateDisposable {

        private VideoRecordController _videoRecordController;

        private SignalBus _signalBus;

        private float timer = 0;
        private CancellationTokenSource cancelTokenSource;

        private float Timer {
            get {
                return timer;
            }
            set {
                if (Mathf.Abs(timer - value) > Mathf.Epsilon) {
                    timer = value;
                }
            }
        }

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        /// <summary>
        /// Record Async
        /// </summary>
        private async void RecordAsync(RecordStartSignal recordStartSignal) {
            cancelTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancelTokenSource.Token;
            try {
                Timer = 0;
                float startTime = Time.time;
                while (Timer < recordStartSignal.RecordingTime.y && !cancellationToken.IsCancellationRequested) {
                    Timer = Time.time - startTime;
                    _signalBus.Fire(new RecordProgressSignal(Timer / recordStartSignal.RecordingTime.y));
                    await Task.Yield();
                }

                _signalBus.Fire(new RecordEndSignal(Timer >= recordStartSignal.RecordingTime.x));

                Timer = 0;
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
            _signalBus.Subscribe<RecordStartSignal>(RecordAsync);
            _signalBus.Subscribe<RecordStopSignal>(Cancel);
        }

        public void LateDispose() {
            _signalBus.Unsubscribe<RecordStartSignal>(RecordAsync);
            _signalBus.Unsubscribe<RecordStopSignal>(Cancel);
        }
    }
}