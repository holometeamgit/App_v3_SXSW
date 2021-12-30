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

        private GameObject _hologram;

        public GameObject Hologram {
            get {
                return _hologram;
            }
        }

        public HologramPlacementSignal(GameObject hologram) {
            _active = true;
            _hologram = hologram;
        }

        public HologramPlacementSignal() {
            _active = false;
        }
    }
}
