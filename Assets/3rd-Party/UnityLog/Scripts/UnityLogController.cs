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
            LogCallBacks.onLog += HandleLog;
        }

        private void OnDisable() {
            Application.logMessageReceived -= HandleLog;
            LogCallBacks.onLog -= HandleLog;
        }

        private void HandleLog(string logString, string stackTrace, LogType type) {
            LogCallBacks.onLog?.Invoke(logString, stackTrace, type, "Default");
        }

        private void HandleLog(string logString, string stackTrace, LogType type, string tag) {
            LogData.AddLog(logString, stackTrace, type, tag);
        }

        private void OnApplicationQuit() {
            LogData.SaveAll();
        }
    }
}
