using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using TMPro;

public class SwitchToProfileOnLogIn : MonoBehaviour {
    [SerializeField]
    Switcher switchToProfile;
    [SerializeField]
    AccountManager accountManager;

    private void OnApplicationFocus(bool focus) {
        if (focus)
            accountManager.QuickLogIn((_, x) => { }, (_, x) => { });
    }

    private void SwitchToProfile() {
        switchToProfile.Switch();
    }

    private void OnEnable() {
        CallBacks.onSignInSuccess += SwitchToProfile;
    }

    private void OnDisable() {
        CallBacks.onSignInSuccess -= SwitchToProfile;
    }
}
