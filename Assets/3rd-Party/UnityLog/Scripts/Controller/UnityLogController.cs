using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Utility.UnityConsole {
    /// <summary>
    /// Controller for Logs
    /// </summary>
    public class UnityLogController : MonoBehaviour {

        private void OnEnable() {
            LogData.Init();
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable() {
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string logString, string stackTrace, LogType type) {
            if (!logString.Contains("#")) {
                LogData.AddLog(logString, stackTrace, type);
            } else {
                int index = logString.IndexOf("#");
                string log = logString.Substring(0, index);
                string tag = logString.Substring(index + 1, logString.Length);
                LogData.AddLog(log, stackTrace, type, tag);
            }
        }
    }
}
