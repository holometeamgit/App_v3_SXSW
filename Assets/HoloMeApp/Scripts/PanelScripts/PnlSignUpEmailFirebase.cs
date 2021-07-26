using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using UnityEngine.UI;

public class PnlSignUpEmailFirebase : MonoBehaviour {
    [SerializeField]
    InputFieldController inputFieldEmail;
    [SerializeField]
    InputFieldController inputFieldPassword;
    [SerializeField]
    InputFieldController inputFieldConfirmPassword;
    [SerializeField]
    Switcher switcherToVerification;
    [SerializeField]
    GameObject LogInLoadingBackground;
    [SerializeField]
    bool isNeedConfirmationPassword = true;
    [SerializeField]
    Toggle isPolicyConfirmed;
    [SerializeField]
    Animator animator;

    private const float COOLDOWN = 0.5f;
    private float nextTimeCanClick = 0;

    public void OnPolicyConfirmationChanged(bool isPolicyConfirmed) {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyEmailOptIn, AnalyticParameters.ParamSignUpMethod, AnalyticsSignUpModeTracker.Instance.SignUpMethodUsed.ToString());
    }

    public void StopAnimation() {
        animator.enabled = false;
    }

    private void Awake() {
        CallBacks.onSignInSuccess += ClearInputFieldData;
    }

    private void SignUp() {
        HelperFunctions.DevLog("Start SignUp");
        if (Time.time < nextTimeCanClick)
            return;
        nextTimeCanClick = (Time.time + COOLDOWN);

        if (!isNeedConfirmationPassword)
            inputFieldConfirmPassword.text = inputFieldPassword.text;

        if (!LocalDataVerification()) {
            return;
        }

        ShowBackground();
        CallBacks.onSignUp?.Invoke(inputFieldEmail.text,
            inputFieldEmail.text,
            inputFieldPassword.text,
            inputFieldConfirmPassword.text);
    }

    private void SignUpCallBack() {
        switcherToVerification.Switch();
        ClearInputFieldData();
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyRegistrationComplete);
    }

    private void ErrorSignUpCallBack(string msg) {
        if (msg.Contains("Password")) {
            inputFieldPassword.ShowWarning(msg);
            inputFieldConfirmPassword.ShowWarning(msg);
        } else {
            inputFieldEmail.ShowWarning(msg);
        }
    }

    private bool LocalDataVerification() {
        if (string.IsNullOrWhiteSpace(inputFieldEmail.text))
            inputFieldEmail.ShowWarning("This field is compulsory");
        if (string.IsNullOrWhiteSpace(inputFieldPassword.text))
            inputFieldPassword.ShowWarning("This field is compulsory");
        if (isNeedConfirmationPassword && string.IsNullOrWhiteSpace(inputFieldConfirmPassword.text))
            inputFieldConfirmPassword.ShowWarning("This field is compulsory");

        return !string.IsNullOrWhiteSpace(inputFieldEmail.text) &&
            !string.IsNullOrWhiteSpace(inputFieldPassword.text) &&
            (!isNeedConfirmationPassword || !string.IsNullOrWhiteSpace(inputFieldConfirmPassword.text));
    }

    private void ClearInputFieldData() {
        inputFieldEmail.text = "";
        inputFieldPassword.text = "";
        inputFieldConfirmPassword.text = "";
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

    private bool CanContinue() {
        return inputFieldEmail.text.Length > 0 && inputFieldPassword.text.Length > 0;
    }

    private void OnEnable() {
        HideBackground();
        CallBacks.onSignUpEMailClick += SignUp;
        CallBacks.onFail += ErrorSignUpCallBack;
        CallBacks.onSignUpSuccess += SignUpCallBack;

        CallBacks.onFail += HideBackground;
        CallBacks.onSignUpSuccess += HideBackground;
        CallBacks.onNeedVerification += HideBackground;

        isPolicyConfirmed.isOn = false;
        isPolicyConfirmed.enabled = false;
        isPolicyConfirmed.enabled = true;
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
