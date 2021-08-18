using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Beem.Extenject.Video {

    /// <summary>
    /// RewindBtn
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class VideoPlayerRewindBtn : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {

        private SignalBus _signalBus;
        private Slider _progress;
        private RewindSignal _rewindSignal = new RewindSignal();
        private StartRewindSignal _startRewindSignal = new StartRewindSignal();
        private FinishRewindSignal _finishRewindSignal = new FinishRewindSignal();

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void Awake() {
            _progress = GetComponent<Slider>();
        }

        public void OnDrag(PointerEventData eventData) {
            _rewindSignal.Percent = _progress.value;
            _signalBus.Fire(_rewindSignal);
        }

        public void OnBeginDrag(PointerEventData eventData) {
            _signalBus.Fire(_startRewindSignal);
        }

        public void OnEndDrag(PointerEventData eventData) {
            _finishRewindSignal.Percent = _progress.value;
            _signalBus.Fire(_finishRewindSignal);
        }
    }
}
