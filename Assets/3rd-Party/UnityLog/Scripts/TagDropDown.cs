using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.Utility.UnityConsole {
    /// <summary>
    /// Change Unity Log Type
    /// </summary>
    [RequireComponent(typeof(Dropdown))]
    public class TagDropDown : MonoBehaviour {

        private Dropdown dropDown;

        private void Awake() {
            dropDown = GetComponent<Dropdown>();
        }

        private void OnEnable() {
            dropDown.onValueChanged.AddListener(OnDropDown);
            LogCallBacks.OnRefresh += Refresh;
        }

        private void OnDisable() {
            dropDown.onValueChanged.RemoveListener(OnDropDown);
            LogCallBacks.OnRefresh -= Refresh;
        }

        private void Refresh() {
            foreach (string item in LogData.GetTags()) {
                Dropdown.OptionData data = new Dropdown.OptionData();
                data.text = item;
                if (!dropDown.options.Contains(data)) {
                    dropDown.options.Add(data);
                }
            }
        }

        private void OnDropDown(int value) {
            LogData.CurrentTag = LogData.GetTags()[value];
        }
    }
}