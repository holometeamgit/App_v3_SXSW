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

        private static Dictionary<LogType, bool> toggleStatus = new Dictionary<LogType, bool>();

        public static void SetToggleLogType(LogType logType, bool status) {
            if (toggleStatus.ContainsKey(logType)) {
                toggleStatus[logType] = status;
            } else {
                toggleStatus.Add(logType, status);
            }
            LogCallBacks.OnRefresh?.Invoke();
        }

        private static bool GetToggleLogType(LogType logType) {
            if (!toggleStatus.ContainsKey(logType)) {
                toggleStatus.Add(logType, false);
            }
            return toggleStatus[logType];
        }

        private static List<UnityLog> _log = new List<UnityLog>();

        /// <summary>
        /// Unity Log
        /// </summary>
        public class UnityLog {
            public LogType Key;
            public string Value;
        }

        public static void Init() {
            string[] PieceTypeNames = Enum.GetNames(typeof(LogType));
            foreach (string item in PieceTypeNames) {
                LogType logType = (LogType)Enum.Parse(typeof(LogType), item);
                AddLog(logType, LoadLogs(logType));
            }
        }

        private static void SaveLogs(LogType logType, string value) {
            PlayerPrefs.SetString(logType.ToString(), value);
        }

        private static string LoadLogs(LogType logType) {
            return PlayerPrefs.GetString(logType.ToString());
        }

        public static void AddLog(LogType logType, string log) {
            UnityLog unityLog = new UnityLog {
                Key = logType,
                Value = log
            };
            _log.Add(unityLog);
        }

        private static void RemoveLog(LogType logType) {
            _log.RemoveAll(x => x.Key == logType);
        }

        /// <summary>
        /// ClearLog
        /// </summary>
        public static void Clear() {

            string[] PieceTypeNames = Enum.GetNames(typeof(LogType));
            foreach (string item in PieceTypeNames) {
                LogType logType = (LogType)Enum.Parse(typeof(LogType), item);
                RemoveLog(logType);
            }
        }

        public static string CurrentLog {
            get {
                string temp = string.Empty;

                foreach (UnityLog item in _log) {
                    if (GetToggleLogType(item.Key)) {
                        temp += item.Value;
                    }
                }
                return temp;
            }
        }

        /// <summary>
        /// Save all item
        /// </summary>
        public static void SaveAll() {

            string[] PieceTypeNames = Enum.GetNames(typeof(LogType));
            foreach (string item in PieceTypeNames) {
                LogType logType = (LogType)Enum.Parse(typeof(LogType), item);
                SaveLogs(logType, _log.Find(x => x.Key == logType).Value);
            }
            PlayerPrefs.Save();
        }

    }

}
