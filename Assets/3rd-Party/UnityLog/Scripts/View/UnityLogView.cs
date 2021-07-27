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

        [Header("Show Last symbols")]
        [SerializeField]
        private int characterLimit = 35000;

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
            int lenght = LogData.GetLog().Length;
            string part = LogData.GetLog().Substring(Mathf.Max(lenght - characterLimit, 0));
            _text.text = part;
        }
    }
}