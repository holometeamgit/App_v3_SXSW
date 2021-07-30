using UnityEngine;
using System;

namespace Beem.Extenject.Hologram {
    /// <summary>
	/// CallBacks
	/// </summary>
    public class HologramCallbacks {
        public static Action<GameObject> onCreateHologram = delegate { };
        public static Action<GameObject> onSelectHologramPrefab = delegate { };
    }
}
