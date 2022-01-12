using System;
using UnityEngine;

namespace WindowManager.Extenject {
    public enum WindowSignalsType {
        OpenWindow,
        OpenPopup,
        CloseWindow
    }

    /// <summary>
    /// Signals for Windows
    /// </summary>
    [Serializable]
    public class WindowSignal {

        [Header("Window Signal Type")]
        [SerializeField]
        private WindowSignalsType _windowSignalsType;

        public WindowSignalsType WindowSignalsType {
            get {
                return _windowSignalsType;
            }
        }

        [Header("Window Id")]
        [SerializeField]
        private string _id;

        public string Id {
            get {
                return _id;
            }
        }

    }
}
