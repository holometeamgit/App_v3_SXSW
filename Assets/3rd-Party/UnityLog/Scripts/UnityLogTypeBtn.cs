using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Beem.Utility.UnityConsole {
    /// <summary>
    /// Change Unity Log Type
    /// </summary>
    public class UnityLogTypeBtn : MonoBehaviour {

        [Header("Unity Log Type")]
        [SerializeField]
        private UnityLogType unityLogType = UnityLogType.FullLogType;

        private LogData _logData = new LogData();

        public void Toggle(bool value) {
            if (value) {
                _logData.CurrentLogType = unityLogType;
            }
        }
    }
}
