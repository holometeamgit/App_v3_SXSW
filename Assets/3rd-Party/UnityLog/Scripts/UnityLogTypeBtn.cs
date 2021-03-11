using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Beem.Utility.UnityConsole {
    /// <summary>
    /// Change Unity Log Type
    /// </summary>
    public class UnityLogTypeBtn : MonoBehaviour {

        [Header("Unity Log Type")]
        [SerializeField]
        private LogType unityLogType = LogType.Log;

        private void OnEnable() {
            Toggle(GetComponent<Toggle>().isOn);
        }

        public void Toggle(bool value) {
            LogData.SetToggleLogType(unityLogType, value);
        }
    }
}
