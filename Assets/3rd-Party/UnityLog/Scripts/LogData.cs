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

        private static Dictionary<LogType, bool> logStatus = new Dictionary<LogType, bool>();

        private static bool isStackTraceStatus = default;
        
        /// <summary>
        /// Stack Trace
        /// </summary>
        public static bool IsStackTraceStatus {
            get {
                return isStackTraceStatus;
            }
            set {
                isStackTraceStatus = value;
                LogCallBacks.OnRefresh?.Invoke();
            }
        }

        public static void SetToggleLogType(List<LogType> logType, bool status) {
            foreach (LogType item in logType) {
                if (logStatus.ContainsKey(item))
                {
                    logStatus[item] = status;
                }
                else
                {
                    logStatus.Add(item, status);
                }
            }

            LogCallBacks.OnRefresh?.Invoke();
        }

        private static bool GetToggleLogType(LogType logType) {
            if (!logStatus.ContainsKey(logType)) {
                logStatus.Add(logType, false);
            }
            return logStatus[logType];
        }

        private static List<UnityLog> _log = new List<UnityLog>();

        /// <summary>
        /// Unity Log
        /// </summary>
        public class UnityLog {
            public LogType Key;
            public string Value;
            public string StackTrace;
            public DateTime Date;
        }

        /// <summary>
        /// Initialise Data
        /// </summary>
        public static void Init()
        {
            string[] PieceTypeNames = Enum.GetNames(typeof(LogType));
            foreach (string item in PieceTypeNames)
            {
                LogType logType = (LogType)Enum.Parse(typeof(LogType), item);
                for (int i = 0; i < GetLogNumber(logType); i++)
                {
                    AddLog(LoadLogs(logType, i));
                }
            }

            
            _log.Sort(delegate (UnityLog x, UnityLog y)
            {
                return DateTime.Compare(x.Date, y.Date);
            });
            
        }

        private static void SaveLogs(UnityLog log) {
            PlayerPrefs.SetString(log.Key+":"+"Log Value", log.Value.ToString());
            DateTimePrefs.Set(log.Key + ":" + "Log Date", log.Date);
            PlayerPrefs.SetString(log.Key + ":" + "Log StackTrace", log.StackTrace.ToString());
        }

        private static void ClearLogs(LogType logType, int logNumber) {
            PlayerPrefs.DeleteKey(logType + logNumber + ":" + "Log Value");
            DateTimePrefs.DeleteKey(logType + logNumber + ":" + "Log Date");
            PlayerPrefs.DeleteKey(logType + logNumber + ":" + "Log StackTrace");
        }

        private static int GetLogNumber(LogType logType) {
            return PlayerPrefs.GetInt(logType + ":" + "Log Number");
        }

        private static void SetLogNumber(LogType logType, int value)
        {
            PlayerPrefs.SetInt(logType + ":" + "Log Number", value);
        }

        private static void ClearLogNumber(LogType logType)
        {
            PlayerPrefs.DeleteKey(logType + ":" + "Log Number");
        }

        private static UnityLog LoadLogs(LogType logType, int logNumber) {
            UnityLog unityLog = new UnityLog
            {
                Key = logType,
                Value = PlayerPrefs.GetString(logType+logNumber + ":" + "Log Value"),
                Date = DateTimePrefs.Get(logType + logNumber + ":" + "Log Date"),
                StackTrace = PlayerPrefs.GetString(logType + logNumber + ":" + "Log StackTrace")
            };
            return unityLog;
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
                StackTrace = stackTrace,
            };
            SetLogNumber(logType, GetLogNumber(logType)+1);
            SaveLogs(unityLog);
            _log.Add(unityLog);
            LogCallBacks.OnRefresh?.Invoke();
        }

        /// <summary>
        /// Add Loading Log
        /// </summary>
        /// <param name="unityLog"></param>
        public static void AddLog(UnityLog unityLog)
        {
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
            foreach (string item in PieceTypeNames)
            {
                LogType logType = (LogType)Enum.Parse(typeof(LogType), item);
                for (int i = 0; i < GetLogNumber(logType); i++)
                {
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
                    if (GetToggleLogType(item.Key) && item.Value!=string.Empty) {
                        temp += RecolorLog.StartRecolor(item.Key);
                        string date = string.Format("{0:D2}:{1:D2}:{2:D2}", item.Date.Hour, item.Date.Minute, item.Date.Second);
                        temp += "[" + date + "]" + "[" + item.Key + "] : " + item.Value + "\n";
                        if (IsStackTraceStatus) {
                            temp += "[StackTrace]" + " : " + item.StackTrace + "\n";
                        }
                        temp += RecolorLog.FinishRecolor(item.Key);
                    }
                }
                return temp;
            }
        }

        /// <summary>
        /// Save all item
        /// </summary>
        public static void SaveAll() {
            PlayerPrefs.Save();
        }

    }

}
