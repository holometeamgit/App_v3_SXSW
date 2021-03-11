using System.Collections;
using System.Collections.Generic;
using NatShare;
using UnityEngine;

namespace Beem.Utility.UnityConsole {

    /// <summary>
    /// Share Controller
    /// </summary>
    public class ShareController : MonoBehaviour {

        private void OnEnable() {
            LogCallBacks.OnShare += Share;
        }

        private void OnDisable() {
            LogCallBacks.OnShare -= Share;
        }

        private void Share() {
#if !UNITY_EDITOR
            using (var payload = new SharePayload()) {
                payload.AddText(LogData.CurrentLog);
            }
#else
            Debug.Log("Share Log:" + LogData.CurrentLog);
#endif
        }

    }
}
