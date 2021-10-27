using Mopsicus.Plugins;
using NiceJson;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.KeyBoard {
    /// <summary>
    /// KeyBoard Constructor
    /// </summary>
    public class KeyBoardConstructor : MonoBehaviour {
        [SerializeField]
        private GameObject _keyboardField;
        [SerializeField]
        private MobileInputField _mobileInputField;
        [SerializeField]
        private KeyBoardPositionView _positionSettingsView;

        public static Action<bool, InputField.OnChangeEvent, InputField.SubmitEvent> onShow = delegate { };
        public static Action<bool, InputField> onInputShow = delegate { };

        private InputField _currentInputField;
        private InputSettings _inputSettings;
        private InputField.OnChangeEvent _currentOnChangeEvent;
        private InputField.SubmitEvent _currentSubmitEvent;

        private void Awake() {
            Construct();
        }

        private void Construct() {
            onShow += Show;
            onInputShow += InputShow;
        }

        private void Show(bool isShown, InputField.OnChangeEvent onChangeEvent, InputField.SubmitEvent submitEvent) {
            _keyboardField.SetActive(isShown);

            _mobileInputField.SetFocus(isShown);

            _positionSettingsView.UpdatePosition();

            if (!isShown) {
                _mobileInputField.InputField.text = string.Empty;
                if (_currentInputField != null) {
                    _mobileInputField.InputField.onValueChanged.RemoveListener(ValueChanged);
                    _mobileInputField.InputField.contentType = _inputSettings.contentType;
                    _mobileInputField.InputField.inputType = _inputSettings.inputType;
                    _mobileInputField.InputField.keyboardType = _inputSettings.keyboardType;
                    _mobileInputField.InputField.characterValidation = _inputSettings.characterValidation;
                    _mobileInputField.InputField.characterLimit = _inputSettings.characterLimit;
                    _currentInputField = null;
                }
                _mobileInputField.InputField.text = string.Empty;
            } else {
                if (_currentOnChangeEvent != null) {
                    _mobileInputField.InputField.onValueChanged.RemoveListener(EventChanged);
                }
                if (_currentSubmitEvent != null) {
                    _mobileInputField.InputField.onEndEdit.RemoveListener(EventSubmit);
                }
                _currentOnChangeEvent = onChangeEvent;
                _currentSubmitEvent = submitEvent;

                if (_currentOnChangeEvent != null) {
                    _mobileInputField.InputField.onValueChanged.AddListener(EventChanged);
                }
                if (_currentSubmitEvent != null) {
                    _mobileInputField.InputField.onEndEdit.AddListener(EventSubmit);
                }
            }
        }

        private void InputShow(bool isShown, InputField inputField) {

            Show(isShown, inputField.onValueChanged, inputField.onEndEdit);

            if (inputField != null && isShown) {
                _currentInputField = inputField;
                _inputSettings = new InputSettings(_mobileInputField.InputField);
                _mobileInputField.InputField.contentType = _currentInputField.contentType;
                _mobileInputField.InputField.inputType = _currentInputField.inputType;
                _mobileInputField.InputField.keyboardType = _currentInputField.keyboardType;
                _mobileInputField.InputField.characterValidation = _currentInputField.characterValidation;
                _mobileInputField.InputField.text = _currentInputField.text;
                _mobileInputField.InputField.characterLimit = _currentInputField.characterLimit;
                _mobileInputField.InputField.onValueChanged.AddListener(ValueChanged);
            }
        }

        private void ValueChanged(string text) {
            if (!string.IsNullOrEmpty(text)) {
                _currentInputField.text = text;
            }
        }

        private void EventChanged(string text) {
            _currentOnChangeEvent?.Invoke(text);
        }

        private void EventSubmit(string text) {
            _currentSubmitEvent?.Invoke(text);
        }

        private void OnDestroy() {
            onShow -= Show;
            onInputShow -= InputShow;
        }

        /// <summary>
        /// structure for previous input Settings
        /// </summary>
        public struct InputSettings {
            public InputField.ContentType contentType { private set; get; }
            public InputField.InputType inputType { private set; get; }
            public TouchScreenKeyboardType keyboardType { private set; get; }
            public InputField.CharacterValidation characterValidation { private set; get; }
            public int characterLimit { private set; get; }

            public InputSettings(InputField inputField) {
                contentType = inputField.contentType;
                inputType = inputField.inputType;
                keyboardType = inputField.keyboardType;
                characterValidation = inputField.characterValidation;
                characterLimit = inputField.characterLimit;
            }
        }
    }
}
