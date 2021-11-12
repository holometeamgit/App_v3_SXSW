using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using TMPro;

public class SwitchToProfileOnLogIn : MonoBehaviour {

    private void SwitchToProfile() {
        UsernameConstructor.OnActivated?.Invoke(true);
        WelcomeConstructor.OnActivated?.Invoke(false);
        SignUpConstructor.OnActivated?.Invoke(false);
        EmailVerificationConstructor.OnActivated?.Invoke(false);
        SignInConstructor.OnActivated?.Invoke(false);
    }

    private void OnEnable() {
        CallBacks.onSignInSuccess += SwitchToProfile;
    }

    private void OnDisable() {
        CallBacks.onSignInSuccess -= SwitchToProfile;
    }
}
