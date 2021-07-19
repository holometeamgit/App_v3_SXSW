using System.Collections;
using System.Collections.Generic;
using System.IO;
using NatShare;
using UnityEngine;

namespace Beem.Utility.UnityConsole {

    /// <summary>
    /// Share Controller
    /// </summary>
    public class ShareController : MonoBehaviour {

        private void OnEnable() {
            LogCallBacks.onShareLog += Share;
        }

        private void OnDisable() {
            LogCallBacks.onShareLog -= Share;
        }

        private void Share() {
#if !UNITY_EDITOR
               new NativeShare().SetText(LogData.GetLog()).Share();
#else
            Debug.Log("Share Log:" + LogData.GetLog());
#endif
        }

    }
}
