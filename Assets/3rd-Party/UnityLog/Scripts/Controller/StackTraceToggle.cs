using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.Utility.UnityConsole {
    /// <summary>
    /// Change StackTrace Status
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class StackTraceToggle : MonoBehaviour {
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

        private void Toggle(bool value) {
            LogData.SetStackTrace(value);
        }
    }
}