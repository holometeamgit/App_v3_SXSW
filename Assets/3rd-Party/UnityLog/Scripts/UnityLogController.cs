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

            string time = string.Format("{0:D2}:{1:D2}:{2:D2}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            string newString = "\n" + time + "\t" + "[" + type + "] : " + logString;
            newString += "\n" + stackTrace;
            switch (type) {
                case LogType.Warning:
                    newString = "<color=yellow>" + newString + "</color> ";
                    break;
                case LogType.Error:
                case LogType.Exception:
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
