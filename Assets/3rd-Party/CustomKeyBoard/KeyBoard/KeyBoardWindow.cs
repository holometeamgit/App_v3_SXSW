using System.Collections.Generic;
using UMI;
using UnityEngine;
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
            KeyBoardConstructor.onHide?.Invoke();
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
        /// Hide Window
        /// </summary>
        public void Hide() {
            _isShown = false;
            gameObject.SetActive(_isShown);
            MobileInputField.SetFocus(_isShown);
            MobileInputField.gameObject.SetActive(_isShown);
            _closeBtn.onClick.RemoveAllListeners();
            _returnBtn.onClick.RemoveAllListeners();
            InputField.onValueChanged.RemoveAllListeners();
            MobileInputField.OnReturnPressedEvent.RemoveAllListeners();
        }

        /// <summary>
        /// Show Window
        /// </summary>
        /// <param name="isShown"></param>
        public void Show(InputField inputField) {
            _isShown = true;
            gameObject.SetActive(_isShown);
            MobileInputField.SetFocus(_isShown);
            MobileInputField.gameObject.SetActive(_isShown);

            UpdateText();
            _returnBtn.onClick.AddListener(() => {
                string text = InputField.text;
                if (InputField.characterLimit == 0) {
                    inputField.onEndEdit?.Invoke(text);
                } else {
                    string customText = text.Substring(0, Mathf.Min(InputField.characterLimit, text.Length));
                    inputField.onEndEdit?.Invoke(customText);
                }
                Return();
            });
            _closeBtn.onClick.AddListener(() => {
                Return();
            });
            MobileInputField.OnReturnPressedEvent.AddListener(() => {
                inputField.onEndEdit?.Invoke(InputField.text);
                Return();
            });
            InputField.onValueChanged.AddListener((text) => {
                UpdateText();
                inputField.onValueChanged?.Invoke(text);
                if (text.Contains("\n")) {
                    _returnBtn.onClick?.Invoke();
                }
            });

        }
    }
}
