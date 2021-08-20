using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Beem.Utility.UnityConsole {
    /// <summary>
    /// Change Unity Log Type
    /// </summary>
    /// 
    [RequireComponent(typeof(Dropdown))]
    public class UnityLogTypeDropDown : MonoBehaviour {

        private Dropdown dropdown;

        private void Awake() {
            dropdown = GetComponent<Dropdown>();
            List<Dropdown.OptionData> datas = new List<Dropdown.OptionData>();
            string[] PieceTypeNames = Enum.GetNames(typeof(LogType));
            foreach (string item in PieceTypeNames) {
                Dropdown.OptionData data = new Dropdown.OptionData();
                data.text = item;
                datas.Add(data);
            }
            dropdown.AddOptions(datas);
            DropDown();
        }

        public void DropDown(int value = 0) {
            LogData.SetLogType((LogType)value);
        }
    }
}
