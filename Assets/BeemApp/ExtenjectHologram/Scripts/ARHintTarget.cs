using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Hologram {
    /// <summary>
    /// AR Hint
    /// </summary>
    public class ARHintTarget : MonoBehaviour {

        [SerializeField]
        private Animator _hintAnimator;

        private bool _arActive;
        private bool _arObjectWasPinched;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void OnEnable() {
            _signalBus.Subscribe<HologramPlacementSignal>(ActivateHologramPlacement);
            _signalBus.Subscribe<ARSessionActivateSignal>(ActivateAR);
            _signalBus.Subscribe<ARPinchSignal>(ActivateARPinch);
        }

        private void OnDisable() {
            _signalBus.Unsubscribe<HologramPlacementSignal>(ActivateHologramPlacement);
            _signalBus.Unsubscribe<ARSessionActivateSignal>(ActivateAR);
            _signalBus.Unsubscribe<ARPinchSignal>(ActivateARPinch);
        }

        /// <summary>
        /// Activate Hologram Placement
        /// </summary>
        /// <param name="signal"></param>
        public void ActivateHologramPlacement(HologramPlacementSignal signal) {
            _hintAnimator.SetBool("Hologram", signal.Active);
        }

        private void ActivateAR(ARSessionActivateSignal signal) {
            _arActive = signal.Active;
            _hintAnimator.SetBool("Active", _arActive && !_arObjectWasPinched);
        }

        private void ActivateARPinch(ARPinchSignal signal) {
            _arObjectWasPinched = signal.Active;
            _hintAnimator.SetBool("Active", _arActive && !_arObjectWasPinched);
        }

    }
}
