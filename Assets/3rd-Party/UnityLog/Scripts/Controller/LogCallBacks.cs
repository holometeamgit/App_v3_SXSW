using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Utility.UnityConsole {

    /// <summary>
    /// Log CallBacks
    /// </summary>
    public class LogCallBacks {
        public static Action onRefreshLog = delegate { };
        public static Action onShareLog = delegate { };
    }
}
