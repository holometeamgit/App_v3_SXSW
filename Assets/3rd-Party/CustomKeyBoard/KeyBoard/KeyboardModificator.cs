
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

        private void OnEnable() {
#if UNITY_EDITOR
            _inputField.enabled = false;
#elif UNITY_IOS
            _inputField.enabled = !_nativeKeyboardOniOS;
            if(!_nativeKeyboardOniOS){
               _inputField.shouldHideMobileInput = _hideMobileInput;
            }
#elif UNITY_ANDROID
            _inputField.enabled = !_nativeKeyboardOnAndroid;
            if(!_nativeKeyboardOnAndroid){
               _inputField.shouldHideMobileInput = _hideMobileInput;
            }
#endif

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
