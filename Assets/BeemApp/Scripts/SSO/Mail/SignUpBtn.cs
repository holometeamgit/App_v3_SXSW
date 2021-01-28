using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Beem.SSO {

    /// <summary>
    /// Button for sign up
    /// </summary>
    public class SignUpBtn : MonoBehaviour, IPointerClickHandler {

        [Header("Profile Name")]
        [SerializeField]
        private InputField _profileName;
        [Header("Email")]
        [SerializeField]
        private InputField _email;
        [Header("Password")]
        [SerializeField]
        private InputField _password;
        [Header("Re-enter password")]
        [SerializeField]
        private InputField _reEnteredPassword;

        public void OnPointerClick(PointerEventData eventData) {
            CallBacks.onSignUp?.Invoke(_profileName.text, _email.text, _password.text, _reEnteredPassword.text);
        }
    }
}
