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
            _signalBus.Subscribe<VideoRecordProgressSignal>(OnProgress);
            _signalBus.Subscribe<VideoRecordEndSignal>(OnEnd);
        }

        private void OnDisable() {
            _signalBus.Unsubscribe<VideoRecordProgressSignal>(OnProgress);
            _signalBus.Unsubscribe<VideoRecordEndSignal>(OnEnd);
        }

        public void OnProgress(VideoRecordProgressSignal recordProgressSignal = null) {
            if (progressBar != null) {
                progressBar.fillAmount = recordProgressSignal != null ? recordProgressSignal.Progress : 0f;
            }
        }

        private void OnEnd() {
            OnProgress();
        }
    }
}
