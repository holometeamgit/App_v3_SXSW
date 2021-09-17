using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Beem.Utility.UnityConsole {

    /// <summary>
    /// LogData for searching current Log System
    /// </summary>
    public static class LogData {

        private static bool _isStackTraceStatus = default;

        private static string _inputKeys = default;

        private static ILog _ILog = new LocalLog();

        private static List<UnityLog> _log = new List<UnityLog>();

        public static event Action onRefreshLog = delegate { };
        public static event Action onRefreshTag = delegate { };

        private static bool isInited = false;

        private const string LOG_TYPE_DATA = "LogTypeData";
        private const string LOG_TAG_DATA = "LogTagData";
        private const string DEFAULT_TAG = "All";

        /// <summary>
        /// Current Log
        /// </summary>
        public static string GetLog() {
            string temp = string.Empty;
            foreach (UnityLog item in _log) {
                if (LogTypeData == item.Key) {
                    if (LogTagData == item.Tag || LogTagData == DEFAULT_TAG) {
                        if (string.IsNullOrEmpty(_inputKeys) || (item.Value.Contains(_inputKeys) || item.StackTrace.Contains(_inputKeys))) {
                            string date = string.Format("{0:D2}:{1:D2}:{2:D2}", item.Date.Hour, item.Date.Minute, item.Date.Second);
                            temp += "[" + date + "]" + "[" + item.Tag + "] : " + item.Value + "\n";
                            if (_isStackTraceStatus) {
                                temp += "[StackTrace]" + " : " + item.StackTrace;
                            }
                        }
                    }
                }
            }
            return temp;
        }

        /// <summary>
        /// Initialise Data
        /// </summary>
        public static void Init() {
            if (!isInited) {
                isInited = true;
                string[] PieceTypeNames = Enum.GetNames(typeof(LogType));
                foreach (string item in PieceTypeNames) {
                    LogType logType = (LogType)Enum.Parse(typeof(LogType), item);
                    for (int i = 0; i < GetLogNumber(logType); i++) {
                        if (LoadLogs(logType, i).Value != string.Empty) {
                            AddLog(LoadLogs(logType, i));
                        }
                    }
                }


                _log.Sort(delegate (UnityLog x, UnityLog y) {
                    return DateTime.Compare(x.Date, y.Date);
                });
            }
        }

        /// <summary>
        /// Select symbols in Logs
        /// </summary>
        /// <param name="inputKeys"></param>
        public static void SetInputKeys(string inputKeys) {
            _inputKeys = inputKeys;
            onRefreshLog?.Invoke();
        }

        /// <summary>
        /// Set Stack Trace
        /// </summary>
        public static void SetStackTrace(bool status) {
            _isStackTraceStatus = status;
            onRefreshLog?.Invoke();
        }

        /// <summary>
        /// Log Type
        /// </summary>
        public static LogType LogTypeData {
            set {
                if ((LogType)PlayerPrefs.GetInt(LOG_TYPE_DATA) != value) {
                    PlayerPrefs.SetInt(LOG_TYPE_DATA, (int)value);
                    onRefreshLog?.Invoke();
                }
            }
            get {
                return (LogType)PlayerPrefs.GetInt(LOG_TYPE_DATA);
            }
        }


        /// <summary>
        /// Tag
        /// </summary>
        public static string LogTagData {
            set {
                if (PlayerPrefs.GetString(LOG_TAG_DATA) != value) {
                    PlayerPrefs.SetString(LOG_TAG_DATA, value);
                    if (_log.Find(x => x.Tag == LogTagData) == null) {
                        onRefreshTag?.Invoke();
                    }

                    onRefreshLog?.Invoke();
                }
            }
            get {
                return PlayerPrefs.GetString(LOG_TAG_DATA, DEFAULT_TAG);
            }
        }

        /// <summary>
        /// Get Tags
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTags() {
            List<string> temp = new List<string>();
            foreach (UnityLog item in _log) {
                if (!temp.Contains(item.Tag)) {
                    temp.Add(item.Tag);
                }
            }
            return temp;
        }

        /// <summary>
        /// Add new Log
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="log"></param>
        /// <param name="stackTrace"></param>
        public static void AddLog(string log, string stackTrace, LogType logType, string tag = "All") {
            UnityLog unityLog = new UnityLog {
                Tag = tag,
                Key = logType,
                Value = log,
                Date = DateTime.Now,
                StackTrace = stackTrace
            };
            SetLogNumber(logType, GetLogNumber(logType) + 1);
            SaveLogs(unityLog);
            _log.Add(unityLog);
            onRefreshLog?.Invoke();
        }

        /// <summary>
        /// Add Loading Log
        /// </summary>
        /// <param name="unityLog"></param>
        public static void AddLog(UnityLog unityLog) {
            _log.Add(unityLog);
            onRefreshLog?.Invoke();
        }

        /// <summary>
        /// Remove Log
        /// </summary>
        /// <param name="logType"></param>
        public static void RemoveLog(LogType logType) {
            _log.RemoveAll(x => x.Key == logType);
        }

        /// <summary>
        /// Clear Log
        /// </summary>
        public static void Clear() {
            string[] PieceTypeNames = Enum.GetNames(typeof(LogType));
            foreach (string item in PieceTypeNames) {
                LogType logType = (LogType)Enum.Parse(typeof(LogType), item);
                for (int i = 0; i < GetLogNumber(logType); i++) {
                    ClearLogs(logType, i);
                }
                ClearLogNumber(logType);
            }
            _log.Clear();
            onRefreshLog?.Invoke();
        }

        private static void ClearLogNumber(LogType logType) {
            _ILog.ClearLogNumber(logType);
        }

        private static void ClearLogs(LogType logType, int logNumber) {
            _ILog.ClearLogs(logType, logNumber);
        }

        private static int GetLogNumber(LogType logType) {
            return _ILog.GetLogNumber(logType);
        }

        private static UnityLog LoadLogs(LogType logType, int logNumber) {
            return _ILog.LoadLogs(logType, logNumber);
        }

        private static void SaveLogs(UnityLog log) {
            _ILog.SaveLogs(log);
        }

        private static void SetLogNumber(LogType logType, int value) {
            _ILog.SetLogNumber(logType, value);
        }

    }

}
