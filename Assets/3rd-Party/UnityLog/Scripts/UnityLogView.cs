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

        private const int MAX_LETTER = 65000;

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
            if (LogData.CurrentLog.Length < MAX_LETTER) {
                _text.text = LogData.CurrentLog;
            } else {

                _text.text = LogData.CurrentLog.Substring(LogData.CurrentLog.Length - MAX_LETTER, LogData.CurrentLog.Length);
            }
        }
    }
}