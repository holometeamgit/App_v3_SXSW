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

        private ARHint _arHint = new ARHint();

        private void OnEnable() {
            _arHint.onHologramPlacement += ActivateHologramPlacement;
        }

        private void OnDisable() {
            _arHint.onHologramPlacement -= ActivateHologramPlacement;
        }

        /// <summary>
        /// Activate Hologram Placement
        /// </summary>
        /// <param name="signal"></param>
        public void ActivateHologramPlacement(HologramPlacementSignal signal) {
            _hintAnimator.SetBool("Hologram", signal.Active);
        }

    }
}
