using System.Collections;
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

    private EmailVerificationTimer emailVerificationTimer = new EmailVerificationTimer();

    private const float COOLDOWN = 0.5f;
    private float nextTimeCanClick = 0;
    private const int TIME_FOR_AUTOHIDINGBG = 5000;

    public void LogIn() {
        HelperFunctions.DevLog("Start login");

        if (Time.time < nextTimeCanClick)
            return;
        nextTimeCanClick = (Time.time + COOLDOWN);

        if (!LocalDataVerification()) {
            return;
        }

        ShowBackground();
        CallBacks.onSignInEMail?.Invoke(inputFieldEmail.text, inputFieldPassword.text);
    }

    /// <summary>
    /// The method do actions after pressing the LogIn button
    /// </summary>
    public void LogInBtnClick() {
        //inputFieldEmail.MobileInputField.SetVisible(false);
        //inputFieldPassword.MobileInputField.SetVisible(false);
        CallBacks.onSignInEMailClick?.Invoke();
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
            inputFieldEmail.SetToDefaultState();
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

        //inputFieldEmail.MobileInputField.SetVisible(false);
        //inputFieldPassword.MobileInputField.SetVisible(false);

        if (EmailVerificationTimer.IsOver) {
            pnlGenericError.ActivateDoubleButton("Email verication",
                string.Format("You have not activated your account via the email, would you like us to send it again? \n {0}", email),
                "Yes",
                "No",
                () => {
                    CallBacks.onEmailVerification?.Invoke();
                    EmailVerificationTimer.Release();
                    //inputFieldEmail.MobileInputField.SetVisible(true);
                    //inputFieldPassword.MobileInputField.SetVisible(true);
                },
                () => {
                    pnlGenericError.gameObject.SetActive(false);
                    //inputFieldEmail.MobileInputField.SetVisible(true);
                    //inputFieldPassword.MobileInputField.SetVisible(true);
                });
        } else {
            pnlGenericError.ActivateSingleButton("Email verication",
               string.Format("You have not activated your account via the email \n {0}", email),
               "Ok",
               () => {
                   pnlGenericError.gameObject.SetActive(false);
                   //inputFieldEmail.MobileInputField.SetVisible(true);
                   //inputFieldPassword.MobileInputField.SetVisible(true);
               });
        }
    }

    private void ClearData() {
        inputFieldEmail.SetToDefaultState();
        inputFieldEmail.text = "";
        inputFieldPassword.SetToDefaultState();
        inputFieldPassword.text = "";
    }

    private void ShowBackground() {
        LogInLoadingBackground.SetActive(true);
    }

    private void AutoHideLoadingBackground(LogInType logInType) {
        AutoHideLoadingBackground();
    }

    private void AutoHideLoadingBackground(string msg) {
        AutoHideLoadingBackground();
    }

    private void AutoHideLoadingBackground() {
        var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task.Delay(TIME_FOR_AUTOHIDINGBG).ContinueWith(_ => HideBackgroundWithDelay(), taskScheduler);
    }

    private void HideBackgroundWithDelay() {
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

    private void OnEnable() {
        HideBackground();
        CallBacks.onSignInEMailClick += LogIn;
        CallBacks.onFail += ErrorLogInCallBack;
        CallBacks.onNeedVerification += NeedVerificationCallback;
        CallBacks.onSignInSuccess += LogInCallBack;
        CallBacks.onFail += AutoHideLoadingBackground;
        CallBacks.onFirebaseSignInSuccess += AutoHideLoadingBackground;

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
        CallBacks.onFail -= AutoHideLoadingBackground;
        CallBacks.onFirebaseSignInSuccess -= AutoHideLoadingBackground;

        CallBacks.onSignInFacebook -= ShowBackground;
        CallBacks.onSignInApple -= ShowBackground;
        CallBacks.onSignInGoogle -= ShowBackground;
        CallBacks.onFail -= HideBackground;
        CallBacks.onNeedVerification -= HideBackground;
        CallBacks.onSignInSuccess -= HideBackground;
    }
}