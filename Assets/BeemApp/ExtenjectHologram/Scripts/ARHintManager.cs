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

        private bool _arActive;
        private bool _arPlanesDetected;
        private bool _arObjectWasCreated;
        private bool _arObjectWasPinched;

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
            if (_arObjectWasCreated != signal.Active) {
                _arObjectWasCreated = signal.Active;
            }
            arPlaneManager.enabled = !_arObjectWasCreated;

            foreach (var plane in arPlaneManager.trackables) {
                plane.gameObject.SetActive(!_arObjectWasCreated);
            }
        }

        protected void ActivateAR(ARSessionActivateSignal signal) {
            if (_arActive != signal.Active) {
                _arActive = signal.Active;
            }
            arSession.enabled = _arActive;

            if (_arActive) {
                arSession.Reset();
            }
        }

        protected void ActivateARPinch(ARPinchSignal signal) {
            if (_arObjectWasPinched != signal.Active) {
                _arObjectWasPinched = signal.Active;
            }
        }

        protected void ActivateARPlanesDetected(ARPlanesDetectedSignal signal) {
            if (_arPlanesDetected != signal.Active) {
                _arPlanesDetected = signal.Active;
            }
        }
    }
}
