using UnityEngine;

namespace Beem.Extenject.Hologram {
    /// <summary>
    /// Hologram Signal
    /// </summary>
    public class HologramSignal : BeemSignal {
        public GameObject Hologram {
            get {
                return _hologram;
            }
        }

        private GameObject _hologram;

        public HologramSignal(GameObject hologram) {
            _hologram = hologram;
        }
    }
}
