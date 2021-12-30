using System.Collections;
using System.Collections.Generic;
using Beem.Extenject.UI;
using UnityEngine;
using Zenject;


namespace Beem.Extenject.Hologram {

    /// <summary>
    /// Open window with appearing hologram
    /// </summary>
    public class HologramWindowCreator : WindowCaller {
        private SignalBus _signalBus;


        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void OnEnable() {
            _signalBus.Subscribe<ARPinchSignal>(CallWindow);
        }

        private void OnDisable() {
            _signalBus.Unsubscribe<ARPinchSignal>(CallWindow);
        }
    }
}