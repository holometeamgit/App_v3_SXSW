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

        private SignalBus _signalBus;

        private void OnEnable() {
            _signalBus.Subscribe<HologramPlacementSignal>(ChangeState);
        }

        private void OnDisable() {
            _signalBus.Unsubscribe<HologramPlacementSignal>(ChangeState);
        }

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void ChangeState(HologramPlacementSignal hologramPlacementSignal) {
            _hintAnimator.SetBool("Hologram", true);
        }

    }
}
