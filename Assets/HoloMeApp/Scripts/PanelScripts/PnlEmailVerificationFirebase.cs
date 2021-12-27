using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using TMPro;
using System.Threading.Tasks;
using System;

public class PnlEmailVerificationFirebase : MonoBehaviour {
    [SerializeField]
    TMP_Text _txtEmail;
    [SerializeField]
    GameObject _goToLogInBtn;
    [SerializeField] TMP_Text _resendMsg;

    [Space]
    [SerializeField]
    private AuthController _authController;

    private const int DELAY_TIME = 5000;
    private const int DELAY_FOR_TIMER = 1000;
    private const string TIMER_TEXT = "You can resend the email in ";

    /// <summary>
    /// The method do actions after pressing the ResendVerification button
    /// </summary>
    public void ResendVerificationBtnClick() {
        CallBacks.onEmailVerification?.Invoke();

        UpdateResendTextAsync();
    }

    private void OnEnable() {
        _txtEmail.text = _authController.GetEmail();

        UpdateResendTextAsync();
        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task.Delay(DELAY_TIME).ContinueWith((_) => {
            _goToLogInBtn.SetActive(true);
        }, taskScheduler);
    }



    private void OnDisable() {
        _goToLogInBtn.SetActive(false);
        EmailVerificationTimer.Cancel();
    }

    /// <summary>
    /// Email Verification To Sign Up
    /// </summary>
    public void EmailVerificationToSignUp() {
        EmailVerificationConstructor.OnActivated?.Invoke(false);
        SignUpConstructor.OnActivated?.Invoke(true);
    }

    /// <summary>
    /// Email Verification To Sign In
    /// </summary>
    public void EmailVerificationToSignIn() {
        EmailVerificationConstructor.OnActivated?.Invoke(false);
        SignInConstructor.OnActivated?.Invoke(true);
    }

    private void OnApplicationFocus(bool focus) {
        _goToLogInBtn.SetActive(true);
    }

    private async void UpdateResendTextAsync() {
        TimeSpan timeSpan = EmailVerificationTimer.GetTimeLeft();
        try {
            while (timeSpan.TotalSeconds > 0 && !EmailVerificationTimer.GetToken().IsCancellationRequested) {
                _resendMsg.text = string.Format(TIMER_TEXT + "{0:D1}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
                await Task.Delay(DELAY_FOR_TIMER);
                timeSpan = EmailVerificationTimer.GetTimeLeft();
            }

        } catch (OperationCanceledException ex) {
            HelperFunctions.DevLog("OperationCanceledException");
        }

    }
}
