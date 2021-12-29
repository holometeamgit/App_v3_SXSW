using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Hologram {

    /// <summary>
    /// AR Hint
    /// </summary>
    public class ARHint {

        public event Action<ARSignal> onActivateAR;
        public event Action<ARPlanesDetectedSignal> onActivateARPlanesDetected;
        public event Action<HologramPlacementSignal> onHologramPlacement;
        public event Action<ARPinchSignal> onActivateARPinch;

        /// <summary>
        /// Activate AR
        /// </summary>
        /// <param name="signal"></param>
        public void ActivateAR(ARSignal signal) {
            onActivateAR?.Invoke(signal);
        }

        /// <summary>
        /// Activate ARPlanesDetected
        /// </summary>
        /// <param name="signal"></param>
        public void ActivateARPlanesDetected(ARPlanesDetectedSignal signal) {
            onActivateARPlanesDetected?.Invoke(signal);
        }

        /// <summary>
        /// Activate HologramPlacement
        /// </summary>
        /// <param name="signal"></param>
        public void ActivateHologramPlacement(HologramPlacementSignal signal) {
            onHologramPlacement?.Invoke(signal);
        }

        /// <summary>
        /// Activate Pinch
        /// </summary>
        /// <param name="signal"></param>
        public void ActivateARPinch(ARPinchSignal signal) {
            onActivateARPinch?.Invoke(signal);
        }
    }
}
