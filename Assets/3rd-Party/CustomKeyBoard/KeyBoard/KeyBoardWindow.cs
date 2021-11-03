using Mopsicus.Plugins;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Beem.KeyBoard {
    /// <summary>
    /// KeyBoard Facade
    /// </summary>
    public class KeyBoardWindow : MonoBehaviour {
        [SerializeField]
        private MobileInputField _mobileInputField;

        public MobileInputField MobileInputField {
            get {
                return _mobileInputField;
            }
        }


        [SerializeField]
        private InputField _inputField;
        public InputField InputField {
            get {
                return _inputField;
            }
        }

        public string Text {
            get {
                return MobileInputField.Text;
            }
            set {
                MobileInputField.Text = value;
            }
        }

        [SerializeField]
        private List<AbstractKeyBoardSettings> _inputFieldSettings = new List<AbstractKeyBoardSettings>();

        private void OnEnable() {
            UpdateText();
            InputField.onValueChanged.AddListener(UpdateText);
        }

        /// <summary>
        /// Return button
        /// </summary>
        public void Return() {
            KeyBoardConstructor.onShow?.Invoke(false, null, null);
        }

        private void OnDisable() {
            InputField.onValueChanged.RemoveListener(UpdateText);
        }

        /// <summary>
        /// Update InputField
        /// </summary>
        /// <param name="text"></param>
        public void UpdateText(string text = "") {
            foreach (AbstractKeyBoardSettings item in _inputFieldSettings) {
                item.RefreshData(InputField);
            }
        }

        /// <summary>
        /// Show Window
        /// </summary>
        /// <param name="isShown"></param>
        public void Show(bool isShown) {
            gameObject.SetActive(isShown);
            MobileInputField.SetFocus(isShown);
        }
    }
}
