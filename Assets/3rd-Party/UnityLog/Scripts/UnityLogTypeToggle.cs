using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Beem.Utility.UnityConsole {
    /// <summary>
    /// Change Unity Log Type
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class UnityLogTypeToggle : MonoBehaviour {

        [Header("Unity Log Type")]
        [SerializeField]
        private List<LogType> unityLogType = new List<LogType>();

        private Toggle toggle;

        private void Awake() {
            toggle = GetComponent<Toggle>();
        }

        private void OnEnable() {
            Toggle(toggle.isOn);
            toggle.onValueChanged.AddListener(Toggle);
        }

        private void OnDisable() {
            toggle.onValueChanged.RemoveListener(Toggle);
        }

        public void Toggle(bool value) {
            LogData.SelectLogTypes(unityLogType, value);
        }
    }
}
