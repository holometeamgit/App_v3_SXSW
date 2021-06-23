using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Beem.Utility.UnityConsole.LogData;

namespace Beem.Utility.UnityConsole {
    /// <summary>
    /// Interface for Data Types and Methods
    /// </summary>
    public interface ILog {
        /// <summary>
        /// Save Logs From Unity UnityLog
        /// </summary>
        /// <param name="unityLog"></param>
        void SaveLogs(UnityLog unityLog);

        /// <summary>
        /// Clear Logs on logNumber and Log Type
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="logNumber"></param>
        void ClearLogs(LogType logType, int logNumber);

        /// <summary>
        /// Get LogNumber
        /// </summary>
        /// <param name="logType"></param>
        /// <returns></returns>

        int GetLogNumber(LogType logType);

        /// <summary>
        /// Set logNumber
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="value"></param>

        void SetLogNumber(LogType logType, int value);

        /// <summary>
        /// Clear Log Number
        /// </summary>
        /// <param name="logType"></param>

        void ClearLogNumber(LogType logType);

        /// <summary>
        /// LoadLogs from Data
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="logNumber"></param>
        /// <returns></returns>

        UnityLog LoadLogs(LogType logType, int logNumber);
    }
}
