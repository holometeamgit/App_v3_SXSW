using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.Utility.UnityConsole {
    /// <summary>
    /// For unity log 
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class UnityLogView : MonoBehaviour {

        private LogData _logData = new LogData();
        private Text _text = default;

        private void Awake() {
            _text = GetComponent<Text>();
            Refresh();
        }

        private void OnEnable() {
            LogCallBacks.OnRefresh += Refresh;
        }

        private void OnDisable() {
            LogCallBacks.OnRefresh -= Refresh;
        }

        private void Refresh() {
            _text.text = _logData.CurrentLog;
        }
    }
}