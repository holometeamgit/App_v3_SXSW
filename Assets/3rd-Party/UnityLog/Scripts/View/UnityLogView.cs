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
        private int _characterLimit = 35000;
        [Header("Subscribe on changing Log")]
        [SerializeField]
        private bool _subscribeOnRefreshLog = default;

        private TextMeshProUGUI _text = default;

        private void Awake() {
            _text = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable() {
            Refresh();
            LogData.onRefreshLog += RefreshOnSubscribe;
        }

        private void OnDisable() {
            LogData.onRefreshLog -= RefreshOnSubscribe;
        }

        private void RefreshOnSubscribe() {
            if (_subscribeOnRefreshLog) {
                Refresh();
            }
        }

        private void Refresh() {
            int lenght = LogData.GetLog().Length;
            string part = LogData.GetLog().Substring(Mathf.Max(lenght - _characterLimit, 0));
            _text.text = part;
        }
    }
}