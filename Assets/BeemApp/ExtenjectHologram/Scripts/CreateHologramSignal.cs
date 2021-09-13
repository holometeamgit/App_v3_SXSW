using UnityEngine;

namespace Beem.Extenject.Hologram {
    /// <summary>
    /// Create Holo Signal
    /// </summary>
    public class CreateHologramSignal : BeemSignal {
        private GameObject _hologram;

        public GameObject Hologram {
            get {
                return _hologram;
            }
        }

        public CreateHologramSignal(GameObject hologram) {
            _hologram = hologram;
        }
    }
}
