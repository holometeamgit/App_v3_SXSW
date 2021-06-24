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
            DropDown();
            dropDown.onValueChanged.AddListener(DropDown);
        }

        private void OnDisable() {
            dropDown.onValueChanged.RemoveListener(DropDown);
        }

        private void DropDown(int value = 0) {
            LogData.SetLogType((LogType)value);
        }
    }
}
