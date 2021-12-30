using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
namespace Beem.Extenject.Hologram {
    /// <summary>
    /// Abstract ARHint
    /// </summary>
    public abstract class AbstractARHint : MonoBehaviour {
        protected bool _arActive;
        protected bool _arPlanesDetected;
        protected bool _arObjectWasCreated;
        protected bool _arObjectWasPinched;

        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        protected void OnEnable() {
            _signalBus.Subscribe<HologramPlacementSignal>(ActivateHologramPlacement);
            _signalBus.Subscribe<ARPlanesDetectedSignal>(ActivateARPlanesDetected);
            _signalBus.Subscribe<ARSessionActivateSignal>(ActivateAR);
            _signalBus.Subscribe<ARPinchSignal>(ActivateARPinch);
        }

        protected void OnDisable() {
            _signalBus.Unsubscribe<HologramPlacementSignal>(ActivateHologramPlacement);
            _signalBus.Unsubscribe<ARPlanesDetectedSignal>(ActivateARPlanesDetected);
            _signalBus.Unsubscribe<ARSessionActivateSignal>(ActivateAR);
            _signalBus.Unsubscribe<ARPinchSignal>(ActivateARPinch);
        }

        protected void ActivateHologramPlacement(HologramPlacementSignal signal) {
            _arObjectWasCreated = signal.Active;
            Refresh();
        }

        protected void ActivateAR(ARSessionActivateSignal signal) {
            _arActive = signal.Active;
            Refresh();
        }

        protected void ActivateARPinch(ARPinchSignal signal) {
            _arObjectWasPinched = signal.Active;
            Refresh();
        }

        protected void ActivateARPlanesDetected(ARPlanesDetectedSignal signal) {
            _arPlanesDetected = signal.Active;
            Refresh();
        }

        protected abstract void Refresh();
    }
}
