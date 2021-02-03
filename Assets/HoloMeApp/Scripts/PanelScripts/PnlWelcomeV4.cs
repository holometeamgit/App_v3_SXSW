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
    PnlGenericError pnlGenericError;

    [SerializeField]
    Switcher switcherToProfile;
    [SerializeField]
    Switcher switcherToLigIn;

    private void Awake() {
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

    /// <summary>
    /// Firebase issue as of Feb 1, 2021. If the user logged in via email,
    /// which is also associated with the user's Facebook account,
    /// then he will no longer be able to log in via Facebook.
    /// </summary>
    private void ShowSpecialErrorFacebookFirebaseMsg(string msg) {
        if (msg.Contains("AccountExistsWithDifferentCredentials")) {
            pnlGenericError.ActivateSingleButton(SpecificFacebookSignInMsg.Title,
                string.Format(SpecificFacebookSignInMsg.SpecificMsg),
                "Continue",
                () => { pnlGenericError.gameObject.SetActive(false); switcherToLigIn.Switch(); }, true);
        }
    }

    private void OnEnable() {
        HideBackground();

        CallBacks.onSignInSuccess += SwitchToProfile;
        CallBacks.onFail += ShowSpecialErrorFacebookFirebaseMsg;

        CallBacks.onSignInFacebook += ShowBackground;
        CallBacks.onSignInApple += ShowBackground;
        CallBacks.onSignInGoogle += ShowBackground;
        CallBacks.onSignInSuccess += HideBackground;
        CallBacks.onFail += HideBackground;
        CallBacks.onNeedVerification += HideBackground;
        webRequestHandler.GetRequest(webRequestHandler.ServerProvidersAPI, EnableFB, (key, body) => { }, null);
    }

    private void OnDisable() {
        CallBacks.onSignInSuccess -= SwitchToProfile;
        CallBacks.onFail -= ShowSpecialErrorFacebookFirebaseMsg;

        CallBacks.onSignInFacebook -= ShowBackground;
        CallBacks.onSignInApple -= ShowBackground;
        CallBacks.onSignInGoogle -= ShowBackground;
        CallBacks.onSignInSuccess -= HideBackground;
        CallBacks.onFail -= HideBackground;
        CallBacks.onNeedVerification -= HideBackground;
    }
}
