using UnityEngine;
using Zenject;

namespace Beem.Extenject.Hologram {

    /// <summary>
    /// Activate/Deactivate AR
    /// </summary>

    public class ARModeCotroller : MonoBehaviour {

        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void OnEnable() {
            _signalBus.Fire(new ARSignal(true));
        }

        private void OnDisable() {
            _signalBus.Fire(new ARSignal(false));
        }
    }
}
