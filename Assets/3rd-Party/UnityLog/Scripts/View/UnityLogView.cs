using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.Utility.UnityConsole {
    /// <summary>
    /// For unity log 
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UnityLogView : MonoBehaviour {

        private TextMeshProUGUI _text = default;

        private void Awake() {
            _text = GetComponent<TextMeshProUGUI>();
            Refresh();
        }

        private void OnEnable() {
            LogData.onRefreshLog += Refresh;
        }

        private void OnDisable() {
            LogData.onRefreshLog -= Refresh;
        }

        private void Refresh() {
            _text.text = LogData.GetLog();
        }
    }
}