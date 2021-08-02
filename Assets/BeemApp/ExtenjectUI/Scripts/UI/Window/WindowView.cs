using UnityEngine;
using Zenject;

namespace Beem.Extenject.UI {
    [RequireComponent(typeof(CanvasGroup))]
    public class WindowView : MonoBehaviour {

        private CanvasGroup _canvasGroup;

        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void OnEnable() {
            _canvasGroup = GetComponent<CanvasGroup>();
            _signalBus.Subscribe<ViewSignal>(ChangeStatus);
        }

        private void OnDisable() {
            _signalBus.Unsubscribe<ViewSignal>(ChangeStatus);
        }

        private void ChangeStatus(ViewSignal viewSignal) {
            _canvasGroup.alpha = viewSignal.Status ? 1 : 0;
        }
    }
}
