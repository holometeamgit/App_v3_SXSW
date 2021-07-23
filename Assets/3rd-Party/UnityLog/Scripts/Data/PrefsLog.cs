using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Utility.UnityConsole {

    /// <summary>
    /// Logs with PlayerPrefs
    /// </summary>
    public class PrefsLog : ILog {
        public void ClearLogNumber(LogType logType) {
            PlayerPrefs.DeleteKey(logType + ":" + "Log Number");
        }

        public void ClearLogs(LogType logType, int logNumber) {
            PlayerPrefs.DeleteKey(logType + logNumber + ":" + "Log Value");
            DateTimePrefs.DeleteKey(logType + logNumber + ":" + "Log Date");
            PlayerPrefs.DeleteKey(logType + logNumber + ":" + "Log StackTrace");
        }

        public int GetLogNumber(LogType logType) {
            return PlayerPrefs.GetInt(logType + ":" + "Log Number");
        }

        public UnityLog LoadLogs(LogType logType, int logNumber) {
            UnityLog unityLog = new UnityLog {
                Key = logType,
                Value = PlayerPrefs.GetString(logType + logNumber + ":" + "Log Value"),
                Date = DateTimePrefs.Get(logType + logNumber + ":" + "Log Date"),
                StackTrace = PlayerPrefs.GetString(logType + logNumber + ":" + "Log StackTrace"),
            };
            return unityLog;
        }

        public void SaveLogs(UnityLog unityLog) {
            PlayerPrefs.SetString(unityLog.Key + ":" + "Log Value", unityLog.Value.ToString());
            DateTimePrefs.Set(unityLog.Key + ":" + "Log Date", unityLog.Date);
            PlayerPrefs.SetString(unityLog.Key + ":" + "Log StackTrace", unityLog.StackTrace.ToString());
            PlayerPrefs.Save();
        }

        public void SetLogNumber(LogType logType, int value) {
            PlayerPrefs.SetInt(logType + ":" + "Log Number", value);
            PlayerPrefs.Save();
        }
    }
}
