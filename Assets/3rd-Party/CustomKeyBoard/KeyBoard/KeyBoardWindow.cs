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
        private KeyBoardPositionView _keyBoardPositionView;

        [SerializeField]
        private Button _returnBtn;

        [SerializeField]
        private Button _closeBtn;

        private bool _isShown = false;

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
        public void UpdateText() {
            foreach (AbstractKeyBoardSettings item in _inputFieldSettings) {
                item.RefreshData(InputField);
            }
        }

        private void OnApplicationPause(bool pause) {
            if (pause && _isShown) {
                Return();
            }
        }

        /// <summary>
        /// Refresh Keyboard Height
        /// </summary>
        /// <param name="height"></param>
        public void RefreshHeight(bool isShown, int height) {
            _keyBoardPositionView.UpdatePosition(isShown, height);

            if (isShown) {
                MobileInputField.SetRectNative();
            }
        }

        /// <summary>
        /// Show Window
        /// </summary>
        /// <param name="isShown"></param>
        public void Show(bool isShown, InputField.OnChangeEvent onChangeEvent, InputField.SubmitEvent submitEvent) {
            _isShown = isShown;
            gameObject.SetActive(isShown);
            MobileInputField.SetFocus(isShown);
            MobileInputField.SetVisible(isShown);

            if (isShown) {
                UpdateText();
                _returnBtn.onClick.AddListener(() => {
                    string text = InputField.text;
                    if (InputField.characterLimit == 0) {
                        submitEvent?.Invoke(text);
                    } else {
                        string customText = text.Substring(0, Mathf.Min(InputField.characterLimit, text.Length));
                        submitEvent?.Invoke(customText);
                    }
                    Return();
                });
                _closeBtn.onClick.AddListener(() => {
                    Return();
                });
                MobileInputField.OnReturnPressedEvent.AddListener(() => {
                    submitEvent?.Invoke(InputField.text);
                    Return();
                });
                InputField.onValueChanged.AddListener((text) => {
                    UpdateText();
                    onChangeEvent?.Invoke(text);
                    if (text.Contains("\n")) {
                        _returnBtn.onClick?.Invoke();
                    }
                });
            } else {
                _closeBtn.onClick.RemoveAllListeners();
                _returnBtn.onClick.RemoveAllListeners();
                InputField.onValueChanged.RemoveAllListeners();
                MobileInputField.OnReturnPressedEvent.RemoveAllListeners();
            }
        }
    }
}
