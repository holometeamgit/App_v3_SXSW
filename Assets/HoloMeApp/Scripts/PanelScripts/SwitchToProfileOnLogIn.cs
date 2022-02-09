using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using TMPro;

public class SwitchToProfileOnLogIn : MonoBehaviour {

    private void SwitchToProfile() {
        CreateUsernameConstructor.OnShow?.Invoke();
        WelcomeConstructor.OnHide?.Invoke();
        SignUpConstructor.OnHide?.Invoke();
        EmailVerificationConstructor.OnHide?.Invoke();
        SignInConstructor.OnHide?.Invoke();
    }

    private void OnEnable() {
        CallBacks.onSignInSuccess += SwitchToProfile;
    }

    private void OnDisable() {
        CallBacks.onSignInSuccess -= SwitchToProfile;
    }
}
