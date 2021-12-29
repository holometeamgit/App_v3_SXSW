using UnityEngine;

namespace Beem.Extenject.Hologram {
    /// <summary>
    /// AR Signal
    /// </summary>
    public class ARSignal : BeemSignal {
        private bool _active;

        public bool Active {
            get {
                return _active;
            }
        }

        public ARSignal(bool active) {
            _active = active;
        }
    }
}
