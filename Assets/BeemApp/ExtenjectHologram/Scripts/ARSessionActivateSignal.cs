using UnityEngine;

namespace Beem.Extenject.Hologram {
    /// <summary>
    /// AR Signal
    /// </summary>
    public class ARSessionActivateSignal : BeemSignal {
        private bool _active;

        public bool Active {
            get {
                return _active;
            }
        }

        public ARSessionActivateSignal(bool active) {
            _active = active;
        }
    }
}
