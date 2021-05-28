using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Beem.Hologram
{
    /// <summary>
	/// CallBacks
	/// </summary>
    public class HologramCallbacks
    {
        public static Action<GameObject> onCreateHologram = delegate { };
        public static Action<GameObject> onSelectHologramPrefab = delegate { };
    }
}
