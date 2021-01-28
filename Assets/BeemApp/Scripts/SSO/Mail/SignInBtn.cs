using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Beem.SSO {

    /// <summary>
    /// Button for forgotting password
    /// </summary>
    public class SignInBtn : MonoBehaviour, IPointerClickHandler {

        [Header("Email")]
        [SerializeField]
        private InputField _email;
        [Header("Password")]
        [SerializeField]
        private InputField _password;

        public void OnPointerClick(PointerEventData eventData) {
            CallBacks.onSignInMail?.Invoke(_email.text, _password.text);
        }
    }
}
