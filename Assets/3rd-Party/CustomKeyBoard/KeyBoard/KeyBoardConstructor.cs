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
        private KeyBoardFacade _keyBoardFacade;
        [SerializeField]
        private MobileInputField _mobileInputField;
        [SerializeField]
        private KeyBoardPositionView _positionSettingsView;

        public static Action<bool, InputField.OnChangeEvent, InputField.SubmitEvent> onShow = delegate { };
        public static Action<bool, UITextField> onUITextShow = delegate { };

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

        private void Show(bool isShown) {
            _keyboardField.SetActive(isShown);

            _mobileInputField.SetFocus(isShown);

            _positionSettingsView.UpdatePosition();
        }


        private void Show(bool isShown, InputField.OnChangeEvent onChangeEvent, InputField.SubmitEvent submitEvent) {

            Show(isShown);

            if (isShown) {
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

        private void ShowWithoutField(bool isShown, InputField.OnChangeEvent onChangeEvent, InputField.SubmitEvent submitEvent) {

            if (_currentUITextField != null) {
                ShowWithField(isShown, _currentUITextField);
                return;
            }

            Debug.LogError(isShown ? "Show without Field" : "Hide without Field");

            Show(isShown, onChangeEvent, submitEvent);

            _mobileInputField.InputField.text = string.Empty;
            _keyBoardFacade.UpdateText();
        }

        private void ShowWithField(bool isShown, UITextField uiTextField) {

            Debug.LogError(isShown ? "Show with Field" : "Hide with Field");

            Show(isShown, uiTextField.onValueChanged, uiTextField.onEndEdit);

            if (isShown) {
                ShowField(uiTextField);
            } else {
                HideField();
            }

            _keyBoardFacade.UpdateText();
        }

        private void ShowField(UITextField uiTextField) {
            _currentUITextField = uiTextField;
            _inputSettings = new KeyBoardSettings(_currentUITextField);
            _mobileInputField.InputField.contentType = _currentUITextField.contentType;
            _mobileInputField.InputField.inputType = _currentUITextField.inputType;
            _mobileInputField.InputField.keyboardType = _currentUITextField.keyboardType;
            _mobileInputField.InputField.characterValidation = _currentUITextField.characterValidation;
            _mobileInputField.InputField.text = _currentUITextField.text;
            _mobileInputField.InputField.characterLimit = _currentUITextField.characterLimit;
            _mobileInputField.InputField.onValueChanged.AddListener(UIValueChanged);
        }

        private void HideField() {
            _mobileInputField.InputField.onValueChanged.RemoveListener(UIValueChanged);
            _mobileInputField.InputField.contentType = _inputSettings.contentType;
            _mobileInputField.InputField.inputType = _inputSettings.inputType;
            _mobileInputField.InputField.keyboardType = _inputSettings.keyboardType;
            _mobileInputField.InputField.characterValidation = _inputSettings.characterValidation;
            _mobileInputField.InputField.characterLimit = _inputSettings.characterLimit;
            _currentUITextField = null;
            _mobileInputField.InputField.text = string.Empty;
        }

        private void UIValueChanged(string text) {
            Debug.LogError("text = " + text);
            _currentUITextField.text = text;
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
