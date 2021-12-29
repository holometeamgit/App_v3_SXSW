using Beem.Extenject.Hologram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Hologram {

    /// <summary>
    /// Ar activate Btn
    /// </summary>
    public class ARActivateBtn : MonoBehaviour {

        private bool _arActive;

        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        public void ActivateARSession() {
            _arActive = !_arActive;
            _signalBus.Fire(new ARSessionActivateSignal(_arActive));

        }
    }
}
