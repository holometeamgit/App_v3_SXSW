using UnityEngine;

namespace Beem.Extenject.Hologram {
    /// <summary>
    /// Create Holo Target Signal
    /// </summary>
    public class TargetPlacementSignal : BeemSignal {
        private Transform _target;

        public Transform Target {
            get {
                return _target;
            }
        }

        public TargetPlacementSignal(Transform target) {
            _target = target;
        }
    }
}
