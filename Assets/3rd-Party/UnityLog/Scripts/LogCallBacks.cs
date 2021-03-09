using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Utility.UnityConsole {

    /// <summary>
    /// Log CallBacks
    /// </summary>
    public class LogCallBacks {
        public static Action OnClear = delegate { };
        public static Action OnRefresh = delegate { };
    }
}
