using UnityEngine;

namespace Beem.Extenject.Hologram {
    /// <summary>
    /// Select Holo Signal
    /// </summary>
    public class SelectHologramSignal : BeemSignal {
        private GameObject _hologram;

        public GameObject Hologram {
            get {
                return _hologram;
            }
        }

        public SelectHologramSignal(GameObject hologram) {
            _hologram = hologram;
        }
    }
}
