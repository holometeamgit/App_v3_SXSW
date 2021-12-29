using UnityEngine;

namespace Beem.Extenject.Hologram {
    /// <summary>
    /// HologramPlacement Signal
    /// </summary>
    public class HologramPlacementSignal : BeemSignal {
        private bool _active;

        public bool Active {
            get {
                return _active;
            }
        }

        public HologramPlacementSignal(bool active) {
            _active = active;
        }
    }
}
