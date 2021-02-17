using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Beem.SSO {
    /// <summary>
    /// Button for sign in via Apple
    /// </summary>
    public class SignInAppleBtn : MonoBehaviour, IPointerClickHandler {
        public void OnPointerClick(PointerEventData eventData) {
            CallBacks.onSignInApple?.Invoke();
        }
    }
}
