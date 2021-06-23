using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Utility.UnityConsole {

    /// <summary>
    /// Local Logs without PlayerPrefs
    /// </summary>
    public class LocalLog : ILog {

        public void ClearLogNumber(LogType logType) {
            LocalPrefs<int>.DeleteKey(logType + ":" + "Log Number");
        }

        public void ClearLogs(LogType logType, int logNumber) {
            LocalPrefs<string>.DeleteKey(logType + logNumber + ":" + "Log Value");
            LocalPrefs<DateTime>.DeleteKey(logType + logNumber + ":" + "Log Date");
            LocalPrefs<string>.DeleteKey(logType + logNumber + ":" + "Log StackTrace");
        }

        public int GetLogNumber(LogType logType) {
            return LocalPrefs<int>.Get(logType + ":" + "Log Number", 0);
        }

        public UnityLog LoadLogs(LogType logType, int logNumber) {
            UnityLog unityLog = new UnityLog {
                Key = logType,
                Value = LocalPrefs<string>.Get(logType + logNumber + ":" + "Log Value", string.Empty),
                Date = LocalPrefs<DateTime>.Get(logType + logNumber + ":" + "Log Date", DateTime.MinValue),
                StackTrace = LocalPrefs<string>.Get(logType + logNumber + ":" + "Log StackTrace", string.Empty),
            };
            return unityLog;
        }

        public void SaveLogs(UnityLog unityLog) {
            LocalPrefs<string>.Set(unityLog.Key + ":" + "Log Value", unityLog.Value);
            LocalPrefs<DateTime>.Set(unityLog.Key + ":" + "Log Date", unityLog.Date);
            LocalPrefs<string>.Set(unityLog.Key + ":" + "Log StackTrace", unityLog.StackTrace);
        }

        public void SetLogNumber(LogType logType, int value) {
            LocalPrefs<int>.Set(logType + ":" + "Log Number", value);
        }
    }
}
