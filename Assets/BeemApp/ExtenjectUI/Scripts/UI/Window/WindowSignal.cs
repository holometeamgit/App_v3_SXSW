using System;
using UnityEngine;

namespace Beem.Extenject.UI {
    public enum WindowSignalsType {
        OpenWindow,
        OpenPopup,
        CloseWindow
    }

    /// <summary>
    /// Signals for Windows
    /// </summary>
    [Serializable]
    public class WindowSignal : BeemSignal {

        [Header("Window Signal Type")]
        [SerializeField]
        private WindowSignalsType _windowSignalsType;

        public WindowSignalsType WindowSignalsType {
            get {
                return _windowSignalsType;
            }
        }

        [Header("Window Signal Type")]
        [SerializeField]
        private WindowObject _windowObject;

        public WindowObject WindowObject {
            get {
                return _windowObject;
            }
        }
    }

}
