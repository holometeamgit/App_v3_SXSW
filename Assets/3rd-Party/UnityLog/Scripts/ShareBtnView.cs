using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Utility.UnityConsole {

    /// <summary>
    /// Share View
    /// </summary>
    public class ShareBtnView : MonoBehaviour {
        private void OnEnable() {
#if !(UNITY_IOS || UNITY_ANDROID)
    gameObject.SetActive(false);
#endif
        }
    }
}