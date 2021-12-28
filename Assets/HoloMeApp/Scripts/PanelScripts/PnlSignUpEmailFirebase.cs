using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using UnityEngine.UI;

public class PnlSignUpEmailFirebase : MonoBehaviour {
    [SerializeField]
    private InputFieldController inputFieldEmail;
    [SerializeField]
    private InputFieldController inputFieldPassword;
    [SerializeField]
    private GameObject LogInLoadingBackground;
    [SerializeField]
    private Animator animator;

    [Space]
    [SerializeField]
    private AccountManager _accountManager;

    private const float COOLDOWN = 0.5f;
    private float nextTimeCanClick = 0;

    public void StopAnimation() {
        animator.enabled = false;
    }

    /// <summary>
    /// The method do actions after pressing the SignUp button
    /// </summary>
    public void SignUpBtnClick() {
        CallBacks.onSignUpEMailClick?.Invoke();
    }

    private void Awake() {
        CallBacks.onSignInSuccess += ClearInputFieldData;
    }

    private void SignUp() {
        HelperFunctions.DevLog("Start SignUp");
        if (Time.time < nextTimeCanClick)
            return;
        nextTimeCanClick = (Time.time + COOLDOWN);

        if (!LocalDataVerification()) {
            return;
        }

        ShowBackground();
        CallBacks.onSignUp?.Invoke(inputFieldEmail.text,
            inputFieldEmail.text,
            inputFieldPassword.text,
            inputFieldPassword.text);
    }

    private void SignUpCallBack() {
        EmailVerificationConstructor.OnActivated?.Invoke(true);
        SignUpConstructor.OnActivated?.Invoke(false);
        ClearInputFieldData();
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyRegistrationComplete);
    }

    /// <summary>
    /// Sign Up To Welcome
    /// </summary>
    public void SignUpToWelcome() {
        WelcomeConstructor.OnActivated?.Invoke(true);
        SignUpConstructor.OnActivated?.Invoke(false);
        _accountManager.LogOut();
    }

    private void ErrorSignUpCallBack(string msg) {
        if (msg.Contains("Password")) {
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

    private void ClearInputFieldData() {
        inputFieldEmail.text = "";
        inputFieldPassword.text = "";
    }

    private void ShowBackground() {
        LogInLoadingBackground.SetActive(true);
    }

    private void HideBackground() {
        LogInLoadingBackground.SetActive(false);
    }

    private void HideBackground(string reason) {
        LogInLoadingBackground.SetActive(false);
    }

    private void OnEnable() {
        HideBackground();
        CallBacks.onSignUpEMailClick += SignUp;
        CallBacks.onFail += ErrorSignUpCallBack;
        CallBacks.onSignUpSuccess += SignUpCallBack;

        CallBacks.onFail += HideBackground;
        CallBacks.onSignUpSuccess += HideBackground;
        CallBacks.onNeedVerification += HideBackground;
    }

    private void OnDisable() {
        CallBacks.onSignUpEMailClick -= SignUp;
        CallBacks.onFail -= ErrorSignUpCallBack;
        CallBacks.onSignUpSuccess -= SignUpCallBack;

        CallBacks.onFail -= HideBackground;
        CallBacks.onSignUpSuccess -= HideBackground;
        CallBacks.onNeedVerification -= HideBackground;
    }
}
