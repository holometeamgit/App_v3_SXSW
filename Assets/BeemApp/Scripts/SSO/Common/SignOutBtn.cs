using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Beem.SSO {

    /// <summary>
    /// Button for Sign Out
    /// </summary>
    public class SignOutBtn : MonoBehaviour, IPointerClickHandler {

        public void OnPointerClick(PointerEventData eventData) {
            CallBacks.onSignOut?.Invoke();
        }
    }
}
