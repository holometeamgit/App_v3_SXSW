using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.KeyBoard {
    /// <summary>
    /// structure for previous input Settings
    /// </summary>
    public struct KeyBoardSettings {

        private InputField.ContentType _contentType;
        public InputField.ContentType ContentType {
            get {
                return _contentType;
            }
        }

        private InputField.InputType _inputType;
        public InputField.InputType InputType {
            get {
                return _inputType;
            }
        }

        private InputField.LineType _lineType;
        public InputField.LineType LineType {
            get {
                return _lineType;
            }
        }

        private TouchScreenKeyboardType _keyboardType;
        public TouchScreenKeyboardType KeyboardType {
            get {
                return _keyboardType;
            }
        }

        private InputField.CharacterValidation _characterValidation;
        public InputField.CharacterValidation CharacterValidation {
            get {
                return _characterValidation;
            }
        }

        private int _characterLimit;
        public int CharacterLimit {
            get {
                return _characterLimit;
            }
        }

        private string _text;
        public string Text {
            get {
                return _text;
            }
        }

        public KeyBoardSettings(InputField inputField) {
            _contentType = inputField.contentType;
            _inputType = inputField.inputType;
            _lineType = inputField.lineType;
            _keyboardType = inputField.keyboardType;
            _characterValidation = inputField.characterValidation;
            _characterLimit = inputField.characterLimit;
            _text = inputField.text;
        }
    }

}