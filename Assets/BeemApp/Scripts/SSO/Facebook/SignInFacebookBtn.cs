using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Beem.SSO {
    /// <summary>
    /// Button for sign in via Facebook
    /// </summary>
    public class SignInFacebookBtn : MonoBehaviour, IPointerClickHandler {
        public void OnPointerClick(PointerEventData eventData) {
            CallBacks.onSignInFacebook?.Invoke();
        }
    }
}
