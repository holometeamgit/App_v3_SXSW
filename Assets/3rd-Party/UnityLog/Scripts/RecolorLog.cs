using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Utility.UnityConsole
{

    /// <summary>
    /// RecolorLog
    /// </summary>
    public static class RecolorLog
    {
        public static string StartRecolor(LogType logType) {
            switch (logType) {
                case LogType.Warning:
                    return "<color=yellow>";
                case LogType.Exception:
                case LogType.Error:
                    return "<color=red>";
                default:
                    return "";
            }
        }

        public static string FinishRecolor(LogType logType)
        {
            switch (logType)
            {
                case LogType.Warning:
                case LogType.Exception:
                case LogType.Error:
                    return "</color> ";
                default:
                    return "";
            }
        }

    }
}
