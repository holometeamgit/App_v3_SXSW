using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Beem.SSO {
    /// <summary>
    /// Email Verification
    /// </summary>
    public class EmailVerificationBtn : MonoBehaviour, IPointerClickHandler {
        public void OnPointerClick(PointerEventData eventData) {
            CallBacks.onEmailVerification?.Invoke();
        }
    }
}
