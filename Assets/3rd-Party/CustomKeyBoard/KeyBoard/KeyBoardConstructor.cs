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

        public static Action<InputField> onShow = delegate { };
        public static Action onHide = delegate { };


        private InputField _currentUITextField;
        private KeyBoardSettings _inputSettings;
        private int _height;
        private Vector2Int _limit = new Vector2Int(826, 1200);

        private void Awake() {
            onShow += Show;
            onHide += Hide;
            MobileInput.OnShowKeyboard += OnShowKeyboard;
        }

        private void OnShowKeyboard(bool isShown, int height) {
            Debug.LogError($"OnShowKeyboard isShown= {isShown}, height = {height}");
            if (isShown) {
                if (height > _limit.x && height < _limit.y) {
                    _height = height;
                } else {
                    _height = _limit.x;
                }
            }
            _keyBoardWindow.RefreshHeight(isShown, _height);

        }

        private void Show(InputField inputField) {

            _keyBoardWindow.Show(inputField);

            SetInputSettings(inputField);

            _keyBoardWindow.UpdateText();
        }

        private void Hide() {

            _keyBoardWindow.Hide();

            RevertInputSettings();

            _keyBoardWindow.UpdateText();
        }

        private void SetInputSettings(InputField inputField) {
            _currentUITextField = inputField;
            _inputSettings = new KeyBoardSettings(_keyBoardWindow.InputField);
            _keyBoardWindow.InputField.contentType = _currentUITextField.contentType;
            _keyBoardWindow.InputField.lineType = _currentUITextField.lineType;
            _keyBoardWindow.InputField.inputType = _currentUITextField.inputType;
            _keyBoardWindow.InputField.keyboardType = _currentUITextField.keyboardType;
            _keyBoardWindow.InputField.characterValidation = _currentUITextField.characterValidation;
            _keyBoardWindow.InputField.characterLimit = _currentUITextField.characterLimit;
            if (_currentUITextField.textComponent.gameObject.activeInHierarchy) {
                _keyBoardWindow.Text = _currentUITextField.text;
            }
        }

        private void RevertInputSettings() {
            _keyBoardWindow.InputField.contentType = _inputSettings.ContentType;
            _keyBoardWindow.InputField.lineType = _inputSettings.LineType;
            _keyBoardWindow.InputField.inputType = _inputSettings.InputType;
            _keyBoardWindow.InputField.keyboardType = _inputSettings.KeyboardType;
            _keyBoardWindow.InputField.characterValidation = _inputSettings.CharacterValidation;
            _keyBoardWindow.InputField.characterLimit = _inputSettings.CharacterLimit;
            _keyBoardWindow.Text = string.Empty;
            _currentUITextField = null;
        }

        private void OnDestroy() {
            onShow -= Show;
            onHide -= Hide;
            MobileInput.OnShowKeyboard -= OnShowKeyboard;
        }
    }
}
