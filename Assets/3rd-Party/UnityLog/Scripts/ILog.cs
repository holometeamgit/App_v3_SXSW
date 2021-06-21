using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Beem.Utility.UnityConsole.LogData;

namespace Beem.Utility.UnityConsole {
    public interface ILog {
        void SaveLogs(UnityLog unityLog);

        void ClearLogs(LogType logType, int logNumber);

        int GetLogNumber(LogType logType);

        void SetLogNumber(LogType logType, int value);

        void ClearLogNumber(LogType logType);

        UnityLog LoadLogs(LogType logType, int logNumber);

        void SaveAll();
    }
}
