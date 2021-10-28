using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.KeyBoard {
    /// <summary>
    /// structure for previous input Settings
    /// </summary>
    public struct KeyBoardSettings {
        public InputField.ContentType contentType { private set; get; }
        public InputField.InputType inputType { private set; get; }
        public InputField.LineType lineType { private set; get; }
        public TouchScreenKeyboardType keyboardType { private set; get; }
        public InputField.CharacterValidation characterValidation { private set; get; }
        public int characterLimit { private set; get; }

        public KeyBoardSettings(UITextField uiTextField) {
            contentType = uiTextField.contentType;
            inputType = uiTextField.inputType;
            lineType = uiTextField.lineType;
            keyboardType = uiTextField.keyboardType;
            characterValidation = uiTextField.characterValidation;
            characterLimit = uiTextField.characterLimit;
        }
    }

}