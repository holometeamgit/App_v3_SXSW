﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using System.Threading.Tasks;

public class PnlLogInEmailFirebase : MonoBehaviour {
    [SerializeField] PnlGenericError pnlGenericError;
    [SerializeField] InputFieldController inputFieldEmail;
    [SerializeField] InputFieldController inputFieldPassword;
    [SerializeField] Switcher switcherToProfile;

    [SerializeField]
    GameObject LogInLoadingBackground;

    private const float COOLDOWN = 0.5f;
    private float nextTimeCanClick = 0;
    private const int TIME_FOR_AUTOHIDINGBG = 5;

    public void LogIn() {
        HelperFunctions.DevLog("Start login");

        if (Time.time < nextTimeCanClick)
            return;
        nextTimeCanClick = (Time.time + COOLDOWN);

        if (!LocalDataVerification()) {
            HelperFunctions.DevLog("LocalDataVerification " + LocalDataVerification());
            return;
        }

        ShowBackground();
        HelperFunctions.DevLog("Start login 2 ");
        CallBacks.onSignInEMail?.Invoke(inputFieldEmail.text, inputFieldPassword.text);
    }

    private void LogInCallBack() {
        switcherToProfile.Switch();
        ClearData();
    }

    private void ErrorLogInCallBack(string msg) {
        if (msg.Contains("AccountExistsWithDifferentCredentials") ||
            msg.Contains("User cancelled login")) { // do nothing
        } else if (msg.Contains("WrongPassword")) {
            inputFieldPassword.ShowWarning(msg);
        } else {
            inputFieldEmail.ShowWarning(msg);
        }
    }

    private bool LocalDataVerification() {
        if (string.IsNullOrWhiteSpace(inputFieldEmail.text))
            inputFieldEmail.ShowWarning("This field is compulsory");
        if (string.IsNullOrWhiteSpace(inputFieldPassword.text))
            inputFieldPassword.ShowWarning("This field is compulsory");

        return !string.IsNullOrWhiteSpace(inputFieldEmail.text) &&
            !string.IsNullOrWhiteSpace(inputFieldPassword.text);
    }

    private void NeedVerificationCallback(string email) {
        inputFieldEmail.ShowWarning("E-mail is not verified");

        pnlGenericError.ActivateDoubleButton("Email verication",
            string.Format("You have not activated your account via the email, would you like us to send it again? {0}", email),
            "Yes",
            "No",
            () => {
                CallBacks.onRequestRepeatVerification?.Invoke();
            },
            () => { pnlGenericError.gameObject.SetActive(false); });
    }

    private void ClearData() {
        inputFieldEmail.SetToDefaultState();
        inputFieldEmail.text = "";
        inputFieldPassword.SetToDefaultState();
        inputFieldPassword.text = "";
    }

    private void ShowBackground() {
        LogInLoadingBackground.SetActive(true);
        HideBackgroundWithDelay();
    }

    private async void HideBackgroundWithDelay() {
        await Task.Delay(TIME_FOR_AUTOHIDINGBG);
        if (LogInLoadingBackground.activeSelf) {
            HideBackground();
        }
    }

    private void HideBackground() {
        LogInLoadingBackground.SetActive(false);
    }

    private void HideBackground(string reason) {
        LogInLoadingBackground.SetActive(false);
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
                () => { pnlGenericError.gameObject.SetActive(false); }, true);
        }
    }

    private void OnEnable() {
        HideBackground();
        CallBacks.onSignInEMailClick += LogIn;
        CallBacks.onFail += ErrorLogInCallBack;
        CallBacks.onNeedVerification += NeedVerificationCallback;
        CallBacks.onSignInSuccess += LogInCallBack;
        CallBacks.onFail += ShowSpecialErrorFacebookFirebaseMsg;

        CallBacks.onSignInFacebook += ShowBackground;
        CallBacks.onSignInApple += ShowBackground;
        CallBacks.onSignInGoogle += ShowBackground;
        CallBacks.onFail += HideBackground;
        CallBacks.onNeedVerification += HideBackground;
        CallBacks.onSignInSuccess += HideBackground;
    }

    private void OnDisable() {
        CallBacks.onSignInEMailClick -= LogIn;
        CallBacks.onFail -= ErrorLogInCallBack;
        CallBacks.onNeedVerification -= NeedVerificationCallback;
        CallBacks.onSignInSuccess -= LogInCallBack;
        CallBacks.onFail -= ShowSpecialErrorFacebookFirebaseMsg;

        CallBacks.onSignInFacebook -= ShowBackground;
        CallBacks.onSignInApple -= ShowBackground;
        CallBacks.onSignInGoogle -= ShowBackground;
        CallBacks.onFail -= HideBackground;
        CallBacks.onNeedVerification -= HideBackground;
        CallBacks.onSignInSuccess -= HideBackground;
    }
}
