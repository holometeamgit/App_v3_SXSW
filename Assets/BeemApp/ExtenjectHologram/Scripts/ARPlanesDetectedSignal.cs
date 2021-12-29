using UnityEngine;

namespace Beem.Extenject.Hologram {
    /// <summary>
    /// ARPlanesDetected Signal
    /// </summary>
    public class ARPlanesDetectedSignal : BeemSignal {
        private bool _active;

        public bool Active {
            get {
                return _active;
            }
        }

        public ARPlanesDetectedSignal(bool active) {
            _active = active;
        }
    }
}
