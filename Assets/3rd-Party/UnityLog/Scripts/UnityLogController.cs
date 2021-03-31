using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Utility.UnityConsole {
    /// <summary>
    /// Controller for Logs
    /// </summary>
    public class UnityLogController : MonoBehaviour {

        private LogData _logData = new LogData();
        private Queue _fullLogQueue = new Queue();
        private Queue _logQueue = new Queue();
        private Queue _warningQueue = new Queue();
        private Queue _errorQueue = new Queue();

        private void ClearLog() {
            _fullLogQueue.Clear();
            _logQueue.Clear();
            _warningQueue.Clear();
            _errorQueue.Clear();
            _logData.Clear();
        }

        private void OnEnable() {
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

            switch (type) {
                case LogType.Warning:
                    newString = "<color=yellow>" + newString + "</color> ";
                    _warningQueue.Enqueue(newString);
                    _fullLogQueue.Enqueue(newString);
                    foreach (string mylog in _warningQueue) {
                        _logData.LogWarning += mylog;
                    }
                    break;
                case LogType.Error:
                case LogType.Exception:
                    newString = "<color=red>" + newString + "</color> ";
                    _errorQueue.Enqueue(newString);
                    _fullLogQueue.Enqueue(newString);
                    newString = "\n<color=red>" + stackTrace + "</color>";
                    _errorQueue.Enqueue(newString);
                    _fullLogQueue.Enqueue(newString);
                    foreach (string mylog in _errorQueue) {
                        _logData.LogError += mylog;
                    }
                    break;
                case LogType.Log:
                default:
                    _logQueue.Enqueue(newString);
                    _fullLogQueue.Enqueue(newString);
                    foreach (string mylog in _logQueue) {
                        _logData.Log += mylog;
                    }
                    break;
            }
            foreach (string mylog in _fullLogQueue) {
                _logData.FullLog += mylog;
            }
        }

        private void OnApplicationQuit() {
            _logData.SaveAll();
        }
    }
}
