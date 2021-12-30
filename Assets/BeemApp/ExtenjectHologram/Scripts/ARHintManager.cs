using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Zenject;

namespace Beem.Extenject.Hologram {
    /// <summary>
    /// AR Hint Manager
    /// </summary>
    public class ARHintManager : MonoBehaviour {

        private ARSession _arSession;

        private ARSession arSession {
            get {
                if (_arSession == null) {
                    _arSession = FindObjectOfType<ARSession>();
                }
                return _arSession;
            }
        }

        private ARPlaneManager _arPlaneManager;

        private ARPlaneManager arPlaneManager {
            get {
                if (_arPlaneManager == null) {
                    _arPlaneManager = FindObjectOfType<ARPlaneManager>();
                }
                return _arPlaneManager;
            }
        }

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
            arPlaneManager.enabled = _arObjectWasCreated;

            foreach (var plane in arPlaneManager.trackables) {
                plane.gameObject.SetActive(_arObjectWasCreated);
            }
        }

        protected void ActivateAR(ARSessionActivateSignal signal) {
            _arActive = signal.Active;
            arSession.enabled = _arActive;

            if (_arActive) {
                arSession.Reset();
            }
        }

        protected void ActivateARPinch(ARPinchSignal signal) {
            _arObjectWasPinched = signal.Active;
        }

        protected void ActivateARPlanesDetected(ARPlanesDetectedSignal signal) {
            _arPlanesDetected = signal.Active;
        }
    }
}
