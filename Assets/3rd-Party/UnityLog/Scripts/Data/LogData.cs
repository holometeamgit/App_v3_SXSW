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

        private static LogType _currentLogType = LogType.Error;

        private static bool _isStackTraceStatus = default;

        private static string _inputKeys = default;

        private static string _currentTag = default;

        private static ILog _ILog = new LocalLog();

        private static List<UnityLog> _log = new List<UnityLog>();

        public static event Action onRefreshLog = delegate { };
        public static event Action onRefreshTag = delegate { };

        private static bool isInited = false;

        /// <summary>
        /// Current Log
        /// </summary>
        public static string GetLog() {
            string temp = string.Empty;
            foreach (UnityLog item in _log) {
                if (_currentLogType == item.Key) {
                    if (_currentTag == item.Tag || _currentTag == "All") {
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
        /// Set Log Type
        /// </summary>
        /// <param name="logType"></param>
        public static void SetLogType(LogType logType) {

            _currentLogType = logType;

            onRefreshLog?.Invoke();
        }

        /// <summary>
        /// Set tag
        /// </summary>
        /// <param name="logType"></param>
        public static void SetTag(string tag) {

            _currentTag = tag;

            if (_log.Find(x => x.Tag == _currentTag) == null) {
                onRefreshTag?.Invoke();
            }

            onRefreshLog?.Invoke();
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
