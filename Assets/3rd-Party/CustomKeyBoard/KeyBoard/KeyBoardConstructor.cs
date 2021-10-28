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
        private KeyBoardWindow _keyBoardWindow;

        public static Action<bool, InputField.OnChangeEvent, InputField.SubmitEvent> onShow = delegate { };
        public static Action<bool, InputField.OnChangeEvent, InputField.SubmitEvent, UITextField> onUITextShow = delegate { };

        private UITextField _currentUITextField;
        private KeyBoardSettings _inputSettings;
        private InputField.OnChangeEvent _currentOnChangeEvent;
        private InputField.SubmitEvent _currentSubmitEvent;

        private void Awake() {
            Construct();
        }

        private void Construct() {
            onShow += ShowWithoutField;
            onUITextShow += ShowWithField;
        }

        private void Show(bool isShown, InputField.OnChangeEvent onChangeEvent, InputField.SubmitEvent submitEvent) {

            _keyBoardWindow.Show(isShown);

            if (isShown) {
                _currentOnChangeEvent = onChangeEvent;
                _currentSubmitEvent = submitEvent;

                if (_currentOnChangeEvent != null) {
                    _keyBoardWindow.InputField.onValueChanged.AddListener(EventChanged);
                }
                if (_currentSubmitEvent != null) {
                    _keyBoardWindow.InputField.onEndEdit.AddListener(EventSubmit);
                }
            } else {
                if (_currentOnChangeEvent != null) {
                    _keyBoardWindow.InputField.onValueChanged.RemoveListener(EventChanged);
                }
                if (_currentSubmitEvent != null) {
                    _keyBoardWindow.InputField.onEndEdit.RemoveListener(EventSubmit);
                }

                _currentOnChangeEvent = null;
                _currentSubmitEvent = null;
            }

        }

        private void ShowWithoutField(bool isShown, InputField.OnChangeEvent onChangeEvent, InputField.SubmitEvent submitEvent) {

            if (!isShown && _currentUITextField != null) {
                ShowWithField(isShown, onChangeEvent, submitEvent, _currentUITextField);
                return;
            }

            Show(isShown, onChangeEvent, submitEvent);

            _keyBoardWindow.Text = string.Empty;
            _keyBoardWindow.UpdateText();
        }

        private void ShowWithField(bool isShown, InputField.OnChangeEvent onChangeEvent, InputField.SubmitEvent submitEvent, UITextField uiTextField) {

            Show(isShown, onChangeEvent, submitEvent);

            if (isShown) {
                ShowField(uiTextField);
            } else {
                HideField();
            }

            _keyBoardWindow.UpdateText();
        }

        private void ShowField(UITextField uiTextField) {
            _currentUITextField = uiTextField;
            _inputSettings = new KeyBoardSettings(_currentUITextField);
            _keyBoardWindow.InputField.contentType = _currentUITextField.ContentType;
            _keyBoardWindow.InputField.inputType = _currentUITextField.InputType;
            _keyBoardWindow.InputField.keyboardType = _currentUITextField.KeyboardType;
            _keyBoardWindow.InputField.characterValidation = _currentUITextField.CharacterValidation;
            _keyBoardWindow.Text = _currentUITextField.Text;
            _keyBoardWindow.InputField.characterLimit = _currentUITextField.CharacterLimit;
        }

        private void HideField() {
            _keyBoardWindow.InputField.contentType = _inputSettings.contentType;
            _keyBoardWindow.InputField.inputType = _inputSettings.inputType;
            _keyBoardWindow.InputField.keyboardType = _inputSettings.keyboardType;
            _keyBoardWindow.InputField.characterValidation = _inputSettings.characterValidation;
            _keyBoardWindow.InputField.characterLimit = _inputSettings.characterLimit;
            _currentUITextField = null;
        }

        private void EventChanged(string text) {
            _currentOnChangeEvent?.Invoke(text);
        }

        private void EventSubmit(string text) {
            _currentSubmitEvent?.Invoke(text);
        }

        private void OnDestroy() {
            onShow -= ShowWithoutField;
            onUITextShow -= ShowWithField;
        }
    }
}
