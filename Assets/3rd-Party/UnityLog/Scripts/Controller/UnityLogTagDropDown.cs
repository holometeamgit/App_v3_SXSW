﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.Utility.UnityConsole {
    /// <summary>
    /// Change Unity Log Tag
    /// </summary>
    /// 
    [RequireComponent(typeof(Dropdown))]
    public class UnityLogTagDropDown : MonoBehaviour {

        private Dropdown dropdown;
        private List<string> PieceTypeNames;

        private void Awake() {
            dropdown = GetComponent<Dropdown>();
            RefreshTags();
        }

        private void OnEnable() {
            LogData.onRefreshTag += RefreshTags;
        }

        private void OnDisable() {
            LogData.onRefreshTag -= RefreshTags;
        }

        private void RefreshTags() {
            dropdown.ClearOptions();
            List<Dropdown.OptionData> datas = new List<Dropdown.OptionData>();
            PieceTypeNames = LogData.GetTags();
            foreach (string item in PieceTypeNames) {
                Dropdown.OptionData data = new Dropdown.OptionData {
                    text = item
                };
                datas.Add(data);
            }
            dropdown.AddOptions(datas);
            dropdown.value = PieceTypeNames.FindIndex(x => x == LogData.LogTagData);
            DropDown(dropdown.value);
        }

        public void DropDown(int value = 0) {
            LogData.LogTagData = PieceTypeNames[value];
        }
    }
}
