using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Beem.Extenject {

    /// <summary>
    /// Signal btn
    /// </summary>
    public class SignalBtn<T> : MonoBehaviour, IPointerClickHandler {

        [Header("Signal Type")]
        [SerializeField]
        protected T _signalType;

        protected SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        public void OnPointerClick(PointerEventData eventData) {
            _signalBus.Fire(_signalType);
        }
    }
}
