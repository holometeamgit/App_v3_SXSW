using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using TMPro;

public class SwitchToProfileOnLogIn : MonoBehaviour {

    private void SwitchToProfile() {
        PnlProfileConstructor._onActivated?.Invoke(true);
        PnlWelcomeConstructor._onActivated?.Invoke(false);
        PnlSignUpEmailConstructor._onActivated?.Invoke(false);
        PnlEmailVerificationConstructor._onActivated?.Invoke(false);
        PnlSignInEmailConstructor._onActivated?.Invoke(false);
    }

    private void OnEnable() {
        CallBacks.onSignInSuccess += SwitchToProfile;
    }

    private void OnDisable() {
        CallBacks.onSignInSuccess -= SwitchToProfile;
    }
}
