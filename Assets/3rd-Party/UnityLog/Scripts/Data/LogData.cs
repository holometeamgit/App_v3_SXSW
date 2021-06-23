using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Beem.Utility.UnityConsole {

    /// <summary>
    /// LogData
    /// </summary>
    public static class LogData {

        private static LogType _currentLogType = LogType.Error;

        private static bool _isStackTraceStatus = default;

        private static ILog _ILog = new PrefsLog();

        private static List<UnityLog> _log = new List<UnityLog>();

        /// <summary>
        /// Stack Trace
        /// </summary>
        public static bool IsStackTraceStatus {
            get {
                return _isStackTraceStatus;
            }
            set {
                _isStackTraceStatus = value;
                LogCallBacks.OnRefresh?.Invoke();
            }
        }

        public static void SelectLogType(LogType logType) {

            _currentLogType = logType;

            LogCallBacks.OnRefresh?.Invoke();
        }

        /// <summary>
        /// Initialise Data
        /// </summary>
        public static void Init() {
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

        private static void SaveLogs(UnityLog log) {
            _ILog.SaveLogs(log);
        }

        private static void ClearLogs(LogType logType, int logNumber) {
            _ILog.ClearLogs(logType, logNumber);
        }

        private static int GetLogNumber(LogType logType) {
            return _ILog.GetLogNumber(logType);
        }

        private static void SetLogNumber(LogType logType, int value) {
            _ILog.SetLogNumber(logType, value);
        }

        private static void ClearLogNumber(LogType logType) {
            _ILog.ClearLogNumber(logType);
        }

        private static UnityLog LoadLogs(LogType logType, int logNumber) {
            return _ILog.LoadLogs(logType, logNumber);
        }

        /// <summary>
        /// Add new Log
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="log"></param>
        /// <param name="stackTrace"></param>
        public static void AddLog(string log, string stackTrace, LogType logType) {
            UnityLog unityLog = new UnityLog {
                Key = logType,
                Value = log,
                Date = DateTime.Now,
                StackTrace = stackTrace
            };
            SetLogNumber(logType, GetLogNumber(logType) + 1);
            SaveLogs(unityLog);
            _log.Add(unityLog);
            LogCallBacks.OnRefresh?.Invoke();
        }

        /// <summary>
        /// Add Loading Log
        /// </summary>
        /// <param name="unityLog"></param>
        public static void AddLog(UnityLog unityLog) {
            _log.Add(unityLog);
            LogCallBacks.OnRefresh?.Invoke();
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
            LogCallBacks.OnRefresh?.Invoke();
        }

        /// <summary>
        /// Current Log
        /// </summary>
        public static string CurrentLog {
            get {
                string temp = string.Empty;
                foreach (UnityLog item in _log) {
                    if (_currentLogType == item.Key) {
                        string date = string.Format("{0:D2}:{1:D2}:{2:D2}", item.Date.Hour, item.Date.Minute, item.Date.Second);
                        temp += "[" + date + "]" + "[" + item.Key + "] : " + item.Value + "\n";
                        if (IsStackTraceStatus) {
                            temp += "[StackTrace]" + " : " + item.StackTrace + "\n";
                        }
                    }
                }
                return temp;
            }
        }

    }

}
