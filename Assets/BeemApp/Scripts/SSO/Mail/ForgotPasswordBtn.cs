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
        public void OnPointerClick(PointerEventData eventData) {
            CallBacks.onResetPasswordClick?.Invoke();
        }
    }
}
