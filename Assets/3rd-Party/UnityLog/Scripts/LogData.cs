using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Beem.Utility.UnityConsole {

    public enum UnityLogType {
        FullLogType,
        LogType,
        WarningType,
        ErrorType
    }

    /// <summary>
    /// LogData
    /// </summary>
    public class LogData {
        private const string LOGS = "Logs";
        private const string FULL_LOGS = "Full Logs";
        private const string ERROR = "Error";
        private const string WARNING = "Warning";
        private const string CURRENT_LOG_TYPE = "Current Log Type";

        public UnityLogType CurrentLogType {
            get {
                return (UnityLogType)Enum.Parse(typeof(UnityLogType), GetTxt(CURRENT_LOG_TYPE, UnityLogType.FullLogType.ToString()));
            }
            set {
                SetTxt(CURRENT_LOG_TYPE, value.ToString());
                LogCallBacks.OnRefresh?.Invoke();
            }
        }

        /// <summary>
        /// ClearLog
        /// </summary>
        public void Clear() {
            FullLog = string.Empty;
            Log = string.Empty;
            LogWarning = string.Empty;
            LogError = string.Empty;
        }

        public string CurrentLog {
            get {
                switch (CurrentLogType) {
                    case UnityLogType.FullLogType:
                        return FullLog;
                    case UnityLogType.LogType:
                        return Log;
                    case UnityLogType.WarningType:
                        return LogWarning;
                    case UnityLogType.ErrorType:
                        return LogError;
                    default:
                        return FullLog;
                }
            }
        }

        public string FullLog {
            get {
                return GetTxt(FULL_LOGS);
            }
            set {
                SetTxt(FULL_LOGS, value);
                LogCallBacks.OnRefresh?.Invoke();
            }
        }

        public string Log {
            get {
                return GetTxt(LOGS);
            }
            set {
                SetTxt(LOGS, value);
            }
        }

        public string LogError {
            get {
                return GetTxt(ERROR);
            }
            set {
                SetTxt(ERROR, value);
            }
        }

        public string LogWarning {
            get {
                return GetTxt(WARNING);
            }
            set {
                SetTxt(WARNING, value);
            }
        }

        private static bool isCompleted = false;

        private Dictionary<string, string> _txt = new Dictionary<string, string>();

        private string GetTxt(string key, string defaultValue = "") {

            if (!_txt.ContainsKey(key)) {
                _txt.Add(key, defaultValue);
            }

            if (!isCompleted) {
                isCompleted = true;
                _txt[key] = Load(key, defaultValue);
            }

            return _txt[key];
        }

        private void SetTxt(string key, string value) {
            if (!_txt.ContainsKey(key)) {
                _txt.Add(key, value);
            } else {
                _txt[key] = Load(key, value);
            }

        }

        public void SaveAll() {
            Save(FULL_LOGS, FullLog);
            Save(LOGS, Log);
            Save(WARNING, LogWarning);
            Save(ERROR, LogError);
            PlayerPrefs.Save();
        }

        public void Save(string key, string value) {
            PlayerPrefs.SetString(key, value);
        }

        public string Load(string key, string defaultValue = "") {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public TextAsset GetLog {
            get {
                string time = string.Format("{0:D2}-{1:D2}-{2:D4}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                string path = Application.dataPath + "/Resources/UnityLogs/" + FULL_LOGS + " - " + time + ".txt";
                File.WriteAllText(path, FullLog);
                return Resources.Load<TextAsset>("UnityLogs/FULL_LOGS" + " - " + time + ".txt");
            }
        }

    }

}
