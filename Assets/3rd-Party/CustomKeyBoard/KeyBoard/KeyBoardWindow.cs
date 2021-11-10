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

        [SerializeField]
        private Button _returnBtn;

        [SerializeField]
        private Button _closeBtn;

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


        /// <summary>
        /// Return button
        /// </summary>
        public void Return() {
            KeyBoardConstructor.onShow?.Invoke(false, null, null);
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
        public void Show(bool isShown, InputField.OnChangeEvent onChangeEvent, InputField.SubmitEvent submitEvent) {
            gameObject.SetActive(isShown);
            MobileInputField.SetFocus(isShown);

            if (isShown) {
                UpdateText();
                _returnBtn.onClick.AddListener(() => {
                    submitEvent?.Invoke(InputField.text);
                    Return();
                });
                _closeBtn.onClick.AddListener(() => {
                    Return();
                });
                InputField.onValueChanged.AddListener((text) => {
                    UpdateText(text);
                    onChangeEvent?.Invoke(text);
                });
            } else {
                _closeBtn.onClick.RemoveAllListeners();
                _returnBtn.onClick.RemoveAllListeners();
                InputField.onValueChanged.RemoveAllListeners();
            }
        }
    }
}
