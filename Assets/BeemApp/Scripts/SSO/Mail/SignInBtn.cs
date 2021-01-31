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

        public void PressSignInEMail() {
            CallBacks.onSignInEMailClick?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData) {
            PressSignInEMail();
        }
    }
}
