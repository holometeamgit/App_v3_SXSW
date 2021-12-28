using UnityEngine;

namespace Beem.Extenject.Hologram {
    /// <summary>
    /// Create Holo Target Signal
    /// </summary>
    public class CreateHologramTargetSignal : BeemSignal {
        private Transform _target;

        public Transform Target {
            get {
                return _target;
            }
        }

        public CreateHologramTargetSignal(Transform target) {
            _target = target;
        }
    }
}
