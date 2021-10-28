using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;

namespace Beem.KeyBoard {
    /// <summary>
    /// UITextField
    /// </summary>
    public class UITextField : MonoBehaviour, IPointerClickHandler {
        [SerializeField]
        private Text textComponent;
        [SerializeField]
        private GameObject placeHolder;

        [SerializeField]
        private InputField.ContentType _contentType;

        public InputField.ContentType ContentType {
            get {
                return _contentType;
            }
        }

        [SerializeField]
        private InputField.InputType _inputType;

        public InputField.InputType InputType {
            get {
                return _inputType;
            }
        }

        [SerializeField]
        private InputField.LineType _lineType;

        public InputField.LineType LineType {
            get {
                return _lineType;
            }
        }

        [SerializeField]
        private TouchScreenKeyboardType _keyboardType;

        public TouchScreenKeyboardType KeyboardType {
            get {
                return _keyboardType;
            }
        }

        [SerializeField]
        private InputField.CharacterValidation _characterValidation;

        public InputField.CharacterValidation CharacterValidation {
            get {
                return _characterValidation;
            }
        }

        [SerializeField]
        private int _characterLimit;

        public int CharacterLimit {
            get {
                return _characterLimit;
            }
        }

        [SerializeField]
        private UnityEvent onClick;

        private void Awake() {
            Text = string.Empty;
        }

        public void OnPointerClick(PointerEventData eventData) {
            onClick?.Invoke();
        }

        public string Text {
            get {
                return textComponent.text;
            }

            set {
                textComponent.text = value;
                placeHolder.SetActive(string.IsNullOrEmpty(textComponent.text));
            }

        }



    }
}