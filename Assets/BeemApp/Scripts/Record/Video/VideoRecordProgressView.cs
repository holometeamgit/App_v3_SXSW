using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Beem.Extenject.Record {

    /// <summary>
    /// Video Progress view
    /// </summary>
    public class VideoRecordProgressView : MonoBehaviour {

        [SerializeField]
        private Image progressBar;

        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void OnEnable() {
            OnProgress();
            _signalBus.Subscribe<RecordProgressSignal>(OnProgress);
            _signalBus.Subscribe<RecordEndSignal>(OnEnd);
        }

        private void OnDisable() {
            _signalBus.Unsubscribe<RecordProgressSignal>(OnProgress);
            _signalBus.Unsubscribe<RecordEndSignal>(OnEnd);
        }

        public void OnProgress(RecordProgressSignal recordProgressSignal = null) {
            if (progressBar != null) {
                progressBar.fillAmount = recordProgressSignal != null ? recordProgressSignal.Progress : 0f;
            }
        }

        private void OnEnd() {
            OnProgress();
        }
    }
}
