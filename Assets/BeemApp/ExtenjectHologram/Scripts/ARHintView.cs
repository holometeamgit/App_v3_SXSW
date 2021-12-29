using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Hologram {

    /// <summary>
    /// AR Hint View
    /// </summary>
    public class ARHintView : MonoBehaviour {

        [SerializeField]
        private GameObject _scan;

        [SerializeField]
        private GameObject _hologramPlacement;

        [SerializeField]
        private GameObject _pinch;

        private SignalBus _signalBus;
        private bool _arActive;
        private bool _arPlanesDetected;
        private bool _arObjectWasCreated;
        private bool _arObjectWasPinched;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void OnEnable() {
            _signalBus.Subscribe<ARSignal>(ActivateAR);
            _signalBus.Subscribe<ARPlanesDetectedSignal>(ActivateARPlanesDetected);
            _signalBus.Subscribe<HologramPlacementSignal>(ActivateHologramPlacement);
            _signalBus.Subscribe<ARPinchSignal>(ActivateARPinch);
        }

        private void OnDisable() {
            _signalBus.Unsubscribe<ARSignal>(ActivateAR);
            _signalBus.Unsubscribe<ARPlanesDetectedSignal>(ActivateARPlanesDetected);
            _signalBus.Unsubscribe<HologramPlacementSignal>(ActivateHologramPlacement);
            _signalBus.Unsubscribe<ARPinchSignal>(ActivateARPinch);
        }

        private void ActivateAR(ARSignal signal) {
            _arActive = signal.Active;
        }

        private void ActivateARPlanesDetected(ARPlanesDetectedSignal signal) {
            _arPlanesDetected = signal.Active;
        }

        private void ActivateHologramPlacement(HologramPlacementSignal signal) {
            _arObjectWasCreated = true;
        }

        private void ActivateARPinch(ARPinchSignal signal) {
            _arObjectWasPinched = signal.Active;
        }

        public void Refresh() {
            _scan.SetActive(_arActive && !_arObjectWasPinched && !_arPlanesDetected && !_arObjectWasCreated);
            _hologramPlacement.SetActive(_arActive && !_arObjectWasPinched && _arPlanesDetected && !_arObjectWasCreated);
            _pinch.SetActive(_arActive && !_arObjectWasPinched && _arPlanesDetected && _arObjectWasCreated);
        }


    }
}
