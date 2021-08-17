using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Utility.UnityConsole {
    /// <summary>
    /// Controller for Logs
    /// </summary>
    public class UnityLogController : MonoBehaviour {

        private const string TAG = "#Tag";

        private void OnEnable() {
            LogData.Init();
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable() {
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string logString, string stackTrace, LogType type) {
            if (!logString.Contains(TAG)) {
                LogData.AddLog(logString, stackTrace, type);
            } else {
                int lastIndex = logString.IndexOf(TAG) + TAG.Length - 1;
                int firstIndex = logString.IndexOf(TAG);
                string log = logString.Substring(0, firstIndex);
                string tag = logString.Substring(lastIndex + 1);
                LogData.AddLog(log, stackTrace, type, tag);
            }
        }
    }
}
