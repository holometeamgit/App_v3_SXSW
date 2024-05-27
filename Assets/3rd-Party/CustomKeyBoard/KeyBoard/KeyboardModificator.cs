using UMI;
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
        private MobileInputField _mobileInputField;
        [SerializeField]
        private bool _hideMobileInput;
        [SerializeField]
        private bool _nativeKeyboardOniOS;
        [SerializeField]
        private bool _nativeKeyboardOnAndroid;

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
            _mobileInputField.enabled = !active;
            if (!active) {
                _inputField.shouldHideMobileInput = _hideMobileInput;
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
