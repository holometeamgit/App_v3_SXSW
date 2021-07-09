using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Utility.UnityConsole {
    /// <summary>
    /// Debug view
    /// </summary>
    public class DebugView : MonoBehaviour {
        private void Awake() {
#if DEV
            gameObject.SetActive(true);
#else
            gameObject.SetActive(false);
#endif
        }
    }
}
