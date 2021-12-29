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

        private bool _arActive;
        private bool _arPlanesDetected;
        private bool _arObjectWasCreated;
        private bool _arObjectWasPinched;

        private ARHint _arHint = new ARHint();

        private void OnEnable() {
            _arHint.onActivateAR += ActivateAR;
            _arHint.onActivateARPlanesDetected += ActivateARPlanesDetected;
            _arHint.onHologramPlacement += ActivateHologramPlacement;
            _arHint.onActivateARPinch += ActivateARPinch;
        }

        private void OnDisable() {
            _arHint.onActivateAR -= ActivateAR;
            _arHint.onActivateARPlanesDetected -= ActivateARPlanesDetected;
            _arHint.onHologramPlacement -= ActivateHologramPlacement;
            _arHint.onActivateARPinch -= ActivateARPinch;
        }

        /// <summary>
        /// Activate AR
        /// </summary>
        /// <param name="signal"></param>
        public void ActivateAR(ARSignal signal) {
            _arActive = signal.Active;
            Refresh();
        }

        /// <summary>
        /// Activate ARPlanesDetected
        /// </summary>
        /// <param name="signal"></param>
        public void ActivateARPlanesDetected(ARPlanesDetectedSignal signal) {
            _arPlanesDetected = signal.Active;
            Refresh();
        }

        /// <summary>
        /// Activate HologramPlacement
        /// </summary>
        /// <param name="signal"></param>
        public void ActivateHologramPlacement(HologramPlacementSignal signal) {
            _arObjectWasCreated = signal.Active;
            Refresh();
        }

        /// <summary>
        /// Activate Pinch
        /// </summary>
        /// <param name="signal"></param>
        public void ActivateARPinch(ARPinchSignal signal) {
            _arObjectWasPinched = signal.Active;
            Refresh();
        }

        private void Refresh() {
            _scan?.SetActive(_arActive && !_arObjectWasPinched && !_arPlanesDetected && !_arObjectWasCreated);
            _hologramPlacement?.SetActive(_arActive && !_arObjectWasPinched && _arPlanesDetected && !_arObjectWasCreated);
            _pinch?.SetActive(_arActive && !_arObjectWasPinched && _arPlanesDetected && _arObjectWasCreated);
        }


    }
}
