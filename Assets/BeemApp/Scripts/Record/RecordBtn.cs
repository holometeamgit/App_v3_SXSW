using UnityEngine;
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

        private SignalBus _signalbus;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalbus = signalBus;
        }

        public void OnPointerDown(PointerEventData eventData) {

            _signalbus.Fire(new RecordStartSignal(recordingTime));
        }

        public void OnPointerUp(PointerEventData eventData) {
            _signalbus.Fire(new RecordStopSignal());
        }

    }
}