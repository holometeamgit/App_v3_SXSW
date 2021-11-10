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
        private int _height;

        private void Awake() {
            Construct();
        }

        private void Construct() {
            onShow += ShowWithoutField;
            onUITextShow += ShowWithField;
            MobileInput.OnShowKeyboard += OnShowKeyboard;
        }

        private void OnShowKeyboard(bool isShown, int height) {
            _height = height;
        }

        private void Show(bool isShown, InputField.OnChangeEvent onChangeEvent, InputField.SubmitEvent submitEvent) {
            _keyBoardWindow.Show(isShown, _height, onChangeEvent, submitEvent);
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
            _inputSettings = new KeyBoardSettings(_keyBoardWindow.InputField);
            _keyBoardWindow.InputField.contentType = _currentUITextField.ContentType;
            _keyBoardWindow.InputField.lineType = _currentUITextField.LineType;
            _keyBoardWindow.InputField.inputType = _currentUITextField.InputType;
            _keyBoardWindow.InputField.keyboardType = _currentUITextField.KeyboardType;
            _keyBoardWindow.InputField.characterValidation = _currentUITextField.CharacterValidation;
            _keyBoardWindow.Text = _currentUITextField.Text;
            _keyBoardWindow.InputField.characterLimit = _currentUITextField.CharacterLimit;
        }

        private void HideField() {
            _keyBoardWindow.InputField.contentType = _inputSettings.ContentType;
            _keyBoardWindow.InputField.lineType = _inputSettings.LineType;
            _keyBoardWindow.InputField.inputType = _inputSettings.InputType;
            _keyBoardWindow.InputField.keyboardType = _inputSettings.KeyboardType;
            _keyBoardWindow.InputField.characterValidation = _inputSettings.CharacterValidation;
            _keyBoardWindow.InputField.characterLimit = _inputSettings.CharacterLimit;
            _currentUITextField = null;
        }

        private void OnDestroy() {
            onShow -= ShowWithoutField;
            onUITextShow -= ShowWithField;
            MobileInput.OnShowKeyboard -= OnShowKeyboard;
        }
    }
}
