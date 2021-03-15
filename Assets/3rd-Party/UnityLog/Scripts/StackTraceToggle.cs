﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.Utility.UnityConsole
{
    /// <summary>
    /// Change Unity Log Type
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class StackTraceToggle : MonoBehaviour
    {
        private Toggle toggle;

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
        }

        private void OnEnable()
        {
            Toggle(toggle.isOn);
            toggle.onValueChanged.AddListener(Toggle);
        }

        private void OnDisable()
        {
            toggle.onValueChanged.RemoveListener(Toggle);
        }

        public void Toggle(bool value)
        {
            LogData.IsStackTraceStatus = value;
        }
    }
}