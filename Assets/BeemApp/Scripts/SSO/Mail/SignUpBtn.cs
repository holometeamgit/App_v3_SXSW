﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Beem.SSO {

    /// <summary>
    /// Button for sign up
    /// </summary>
    public class SignUpBtn : MonoBehaviour, IPointerClickHandler {

        public void SignUp() {
            Debug.Log("onSignUpEMailClick");
            CallBacks.onSignUpEMailClick?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData) {
            SignUp();
        }
    }
}
