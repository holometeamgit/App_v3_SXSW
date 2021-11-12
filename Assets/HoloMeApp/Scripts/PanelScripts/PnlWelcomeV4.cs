using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Beem.SSO;
using System.Threading.Tasks;
//TODO all this class temp
public class PnlWelcomeV4 : MonoBehaviour {
    [SerializeField]
    GameObject LogInBackground;

    private const int TIME_FOR_AUTOHIDINGBG = 10000;
    private int HIDE_BACKGROUND_DELAY_TIME = 500;

    private void ShowBackground() {
        HelperFunctions.DevLog("Show Background");
        LogInBackground.SetActive(true);
    }

    private void HideBackground() {
        HelperFunctions.DevLog("Hide Background");
        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task.Delay(HIDE_BACKGROUND_DELAY_TIME).ContinueWith((_) => LogInBackground.SetActive(false), taskScheduler);
    }

    private void HideBackground(string reason) {
        HideBackground();
    }

    private void SwitchToProfile() {
        HelperFunctions.DevLog("Welcome SwitchToProfile");
        UsernameConstructor.OnActivated?.Invoke(true);
        WelcomeConstructor.OnActivated?.Invoke(false);
    }

    /// <summary>
    /// Switch To Sign Up
    /// </summary>
    public void SwitchToSignUp() {
        HelperFunctions.DevLog("Welcome SwitchToSignUp");
        SignUpConstructor.OnActivated?.Invoke(true);
        WelcomeConstructor.OnActivated?.Invoke(false);
    }

    /// <summary>
    /// Switch To Sign In
    /// </summary>
    public void SwitchToSignIn() {
        HelperFunctions.DevLog("Welcome SwitchToSignIn");
        SignInConstructor.OnActivated?.Invoke(true);
        WelcomeConstructor.OnActivated?.Invoke(false);
    }

    private void OnEnable() {
        HideBackground();

        CallBacks.onSignInSuccess += SwitchToProfile;

        CallBacks.onSignInFacebook += ShowBackground;
        CallBacks.onSignInApple += ShowBackground;
        CallBacks.onSignInGoogle += ShowBackground;
        CallBacks.onFail += HideBackground;
        CallBacks.onNeedVerification += HideBackground;
    }

    private void OnDisable() {
        CallBacks.onSignInSuccess -= SwitchToProfile;

        CallBacks.onSignInFacebook -= ShowBackground;
        CallBacks.onSignInApple -= ShowBackground;
        CallBacks.onSignInGoogle -= ShowBackground;
        CallBacks.onFail -= HideBackground;
        CallBacks.onNeedVerification -= HideBackground;

        HideBackground();
    }
}
