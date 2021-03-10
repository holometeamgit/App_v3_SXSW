using System.Collections;
using System.Collections.Generic;
using NatShare;
using UnityEngine;

namespace Beem.Utility.UnityConsole {

    /// <summary>
    /// Share Controller
    /// </summary>
    public class ShareController : MonoBehaviour {


        private LogData _logData = new LogData();

        private void OnEnable() {
            LogCallBacks.OnShare += Share;
        }

        private void OnDisable() {
            LogCallBacks.OnShare -= Share;
        }

        private void Share() {
            using (var payload = new SharePayload()) {
                payload.AddText(_logData.FullLog);
            }
        }

    }
}
