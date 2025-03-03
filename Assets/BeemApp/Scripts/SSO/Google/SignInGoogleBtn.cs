﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Beem.SSO {
    /// <summary>
    /// Button for sign in via Google
    /// </summary>
    public class SignInGoogleBtn : MonoBehaviour, IPointerClickHandler {
        public void OnPointerClick(PointerEventData eventData) {
            CallBacks.onSignInGoogle?.Invoke();
        }
    }
}
