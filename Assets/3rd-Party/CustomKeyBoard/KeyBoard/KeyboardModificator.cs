
using Mopsicus.Plugins;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Beem.KeyBoard {
    /// <summary>
    /// KeyBoard Actions
    /// </summary>
    public class KeyboardModificator : MonoBehaviour, IPointerClickHandler {

        [SerializeField]
        private InputField _inputField;
        [SerializeField]
        private bool _hideMobileInput;
        [SerializeField]
        private bool _nativeKeyboardOniOS;
        [SerializeField]
        private bool _nativeKeyboardOnAndroid;

        private MobileInputField mobileInputField;

        private void OnEnable() {
#if UNITY_EDITOR
            ActivateCustomField(true);
#elif UNITY_IOS
            ActivateCustomField(_nativeKeyboardOniOS);
#elif UNITY_ANDROID
            ActivateCustomField(_nativeKeyboardOnAndroid);
#endif
        }

        private void ActivateCustomField(bool active) {
            _inputField.enabled = !active;
            if (!active) {
                _inputField.shouldHideMobileInput = _hideMobileInput;
                if (mobileInputField = null) {
                    mobileInputField = gameObject.AddComponent<MobileInputField>();
                    mobileInputField.IsWithDoneButton = false;
                    mobileInputField.IsWithClearButton = false;
                    mobileInputField.OnReturnPressedEvent.AddListener(() => _inputField.onEndEdit?.Invoke(_inputField.text));
                }
            } else {
                if (mobileInputField != null) {
                    mobileInputField.OnReturnPressedEvent.RemoveListener(() => _inputField.onEndEdit?.Invoke(_inputField.text));
                    mobileInputField.enabled = false;
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
#if UNITY_EDITOR
            KeyBoardConstructor.onShow?.Invoke(_inputField);
#elif UNITY_IOS
            if(_nativeKeyboardOniOS){
                KeyBoardConstructor.onShow?.Invoke(_inputField);
            }
#elif UNITY_ANDROID
            if(_nativeKeyboardOnAndroid){
                KeyBoardConstructor.onShow?.Invoke(_inputField);
            }
#endif
        }
    }
}
