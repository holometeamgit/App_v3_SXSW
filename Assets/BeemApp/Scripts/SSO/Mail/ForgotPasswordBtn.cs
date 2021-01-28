using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Beem.SSO {

    /// <summary>
    /// Button for forgotting password
    /// </summary>
    public class ForgotPasswordBtn : MonoBehaviour, IPointerClickHandler {

        [Header("User Name")]
        [SerializeField]
        private InputField _inputField;

        public void OnPointerClick(PointerEventData eventData) {
            if (!_inputField.text.Contains("@")) {
                CallBacks.onFail?.Invoke("Empty Mail");
                return;
            }

            CallBacks.onForgotAccount?.Invoke(_inputField.text);
        }
    }
}
