using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Beem.SSO;
//TODO all this class temp
public class PnlWelcomeV4 : MonoBehaviour {
    [Serializable]
    public class Providers {
        public List<string> providers;

        public Providers() {
            providers = new List<string>();
        }
    }

    [SerializeField]
    WebRequestHandler webRequestHandler;
    [SerializeField]
    GameObject LogInFBGO;
    [SerializeField]
    GameObject LogInAppleGO;
    [SerializeField]
    GameObject LogInGoogleGO;
    [SerializeField]
    GameObject LogInBackground;

    [SerializeField]
    Switcher switcherToProfile;

    private void OnEnable() {
        HideBackground();
#if !UNITY_IOS
        LogInAppleGO.SetActive(false);
#endif
        CallBacks.onSignInFacebook += ShowBackground;
        CallBacks.onSignInApple += ShowBackground;
        CallBacks.onSignInGoogle += ShowBackground;
        CallBacks.onSignInSuccess += HideBackground;
        CallBacks.onSignInSuccess += SwitchToProfile;
        CallBacks.onFail += HideBackground;
        webRequestHandler.GetRequest(webRequestHandler.ServerProvidersAPI, EnableFB, (key, body) => { }, null);
    }

    private void OnDisable() {
        CallBacks.onSignInFacebook -= ShowBackground;
        CallBacks.onSignInApple -= ShowBackground;
        CallBacks.onSignInGoogle -= ShowBackground;
        CallBacks.onSignInSuccess -= HideBackground;
        CallBacks.onSignInSuccess -= SwitchToProfile;
        CallBacks.onFail -= HideBackground;
    }

    private void EnableFB(long key, string body) {
        try {
            Providers providers = JsonUtility.FromJson<Providers>(body);
            if (providers != null) {
                LogInFBGO.SetActive(providers.providers.Contains("fb"));
#if UNITY_IOS
                LogInAppleGO.SetActive(providers.providers.Contains("apple"));
#endif
                LogInGoogleGO.SetActive(providers.providers.Contains("google"));
            }
        } catch (Exception e) { }
    }

    private void ShowBackground() {
        LogInBackground.SetActive(true);
    }

    private void HideBackground() {
        LogInBackground.SetActive(false);
    }

    private void HideBackground(string reason) {
        LogInBackground.SetActive(false);
    }

    private void SwitchToProfile() {
        switcherToProfile.Switch();
    }
}
