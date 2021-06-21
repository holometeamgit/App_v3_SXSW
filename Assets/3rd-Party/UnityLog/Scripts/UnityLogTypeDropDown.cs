using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Beem.Utility.UnityConsole {
    /// <summary>
    /// Change Unity Log Type
    /// </summary>
    [RequireComponent(typeof(Dropdown))]
    public class UnityLogTypeDropDown : MonoBehaviour {

        private Dropdown dropDown;

        private void Awake() {
            dropDown = GetComponent<Dropdown>();
        }

        private void OnEnable() {
            dropDown.onValueChanged.AddListener(DropDown);
        }

        private void OnDisable() {
            dropDown.onValueChanged.RemoveListener(DropDown);
        }

        public void DropDown(int value) {
            LogData.SelectLogType((LogType)value);
        }
    }
}
