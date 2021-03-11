using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Utility.UnityConsole {
    /// <summary>
    /// Controller for Logs
    /// </summary>
    public class UnityLogController : MonoBehaviour {

        private void ClearLog() {
            LogData.Clear();
        }

        private void OnEnable() {
            LogData.Init();
            Application.logMessageReceived += HandleLog;
            LogCallBacks.OnClear += ClearLog;
        }

        private void OnDisable() {
            Application.logMessageReceived -= HandleLog;
            LogCallBacks.OnClear -= ClearLog;
        }

        private void HandleLog(string logString, string stackTrace, LogType type) {
            string newString = "[" + type + "] : " + logString + "\n";
            switch (type) {
                case LogType.Warning:
                    newString = "<color=yellow>" + newString + "</color> ";
                    break;
                case LogType.Exception:
                    newString += "[StackTrace] : " + stackTrace + "\n";
                    newString = "<color=red>" + newString + "</color> ";
                    break;
            }

            LogData.AddLog(type, newString);

        }

        private void OnApplicationQuit() {
            LogData.SaveAll();
        }
    }
}
