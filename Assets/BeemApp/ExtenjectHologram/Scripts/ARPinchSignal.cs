using UnityEngine;

namespace Beem.Extenject.Hologram {
    /// <summary>
    /// ARPinch Signal
    /// </summary>
    public class ARPinchSignal : BeemSignal {
        private bool _active;

        public bool Active {
            get {
                return _active;
            }
        }

        public ARPinchSignal(bool active) {
            _active = active;
        }
    }
}
