using UnityEngine;

namespace Beem.Extenject.Hologram {
    /// <summary>
    /// Create Holo Signal
    /// </summary>
    public class HologramPlacementSignal : BeemSignal {
        private GameObject _hologram;

        public GameObject Hologram {
            get {
                return _hologram;
            }
        }

        public HologramPlacementSignal(GameObject hologram) {
            _hologram = hologram;
        }
    }
}
