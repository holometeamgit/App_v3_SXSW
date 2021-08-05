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
    public class RecordBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

        [Header("Recording Time")]
        [SerializeField]
        private Vector2 recordingTime = new Vector2(2, 15);

        [SerializeField]
        private UnityEvent<float> onRecordProgress;

        private VideoRecordController _videoRecordController;

        private float timer = 0;
        private CancellationTokenSource cancelTokenSource;
        private bool pressed = false;

        private float Timer {
            get {
                return timer;
            }
            set {
                if (Mathf.Abs(timer - value) > Mathf.Epsilon) {
                    timer = value;
                    onRecordProgress?.Invoke(timer / (recordingTime.y));
                }
            }
        }

        [Inject]
        public void Construct(VideoRecordController videoRecordController) {
            _videoRecordController = videoRecordController;
        }

        public void OnPointerDown(PointerEventData eventData) {
            pressed = true;
            RecordAsync();
        }

        public void OnPointerUp(PointerEventData eventData) {
            pressed = false;
        }

        /// <summary>
        /// Record Async
        /// </summary>
        public async void RecordAsync() {
            cancelTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancelTokenSource.Token;
            try {
                _videoRecordController.OnRecordStart();
                Timer = 0;
                float startTime = Time.time;
                while (Timer < recordingTime.y && pressed && !cancellationToken.IsCancellationRequested) {
                    Timer = Time.time - startTime;
                    await Task.Yield();
                }
                if (Timer < recordingTime.x) {
                    _videoRecordController.OnRecordFail();
                } else {
                    _videoRecordController.OnRecordStop();
                }
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
    }
}