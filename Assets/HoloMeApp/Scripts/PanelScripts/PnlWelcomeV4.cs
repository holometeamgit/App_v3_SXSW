﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Beem.SSO;
using System.Threading.Tasks;
//TODO all this class temp
public class PnlWelcomeV4 : MonoBehaviour {
    [SerializeField]
    GameObject LogInBackground;
    [SerializeField]
    PnlGenericError pnlGenericError;

    [SerializeField]
    Switcher switcherToProfile;
    [SerializeField]
    Switcher switcherToLigIn;

    private const int TIME_FOR_AUTOHIDINGBG = 10000;
    private int HIDE_BACKGROUND_DELAY_TIME = 500;

    private DeepLinkHandler deepLinkHandler;
    private const string IS_CHECKED_PUSH_KEY = "pushIsChecked";


    private void Awake() {
        deepLinkHandler = FindObjectOfType<DeepLinkHandler>();
    }

    private void Start() {
#if UNITY_ANDROID
        if (deepLinkHandler != null && !PlayerPrefs.HasKey(IS_CHECKED_PUSH_KEY)) {
            deepLinkHandler.CheckSettings();
            PlayerPrefs.SetInt(IS_CHECKED_PUSH_KEY, 1);
            PlayerPrefs.Save();
        }
#endif
    }

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
        CallBacks.onFail += HideBackground;
        CallBacks.onNeedVerification += HideBackground;               
    }

    private void OnDisable() {
        CallBacks.onSignInSuccess -= SwitchToProfile;
        CallBacks.onFail -= ShowSpecialErrorFacebookFirebaseMsg;

        CallBacks.onSignInFacebook -= ShowBackground;
        CallBacks.onSignInApple -= ShowBackground;
        CallBacks.onSignInGoogle -= ShowBackground;
        CallBacks.onFail -= HideBackground;
        CallBacks.onNeedVerification -= HideBackground;

        HideBackground();
    }
}
