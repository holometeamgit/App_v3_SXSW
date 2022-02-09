using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using System.Threading.Tasks;
using Zenject;
using WindowManager.Extenject;

public class PnlLogInEmailFirebase : MonoBehaviour {

    [SerializeField] InputFieldController inputFieldEmail;
    [SerializeField] InputFieldController inputFieldPassword;

    [SerializeField]
    GameObject LogInLoadingBackground;

    private AccountManager _accountManager;

    private EmailVerificationTimer emailVerificationTimer = new EmailVerificationTimer();

    private const float COOLDOWN = 0.5f;
    private float nextTimeCanClick = 0;
    private const int TIME_FOR_AUTOHIDINGBG = 5000;

    [Inject]
    public void Construct(AccountManager accountManager) {
        _accountManager = accountManager;
    }

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
        CallBacks.onSignInEMailClick?.Invoke();
    }

    private void LogInCallBack() {
        SignInToProfile();
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
        inputFieldEmail.MobileInputField.gameObject.SetActive(false);
        inputFieldPassword.MobileInputField.gameObject.SetActive(false);
        if (EmailVerificationTimer.IsOver) {
            GeneralPopUpData.ButtonData funcButton = new GeneralPopUpData.ButtonData("Yes", () => {
                CallBacks.onEmailVerification?.Invoke();
                EmailVerificationTimer.Release();
                inputFieldEmail.MobileInputField.gameObject.SetActive(true);
                inputFieldPassword.MobileInputField.gameObject.SetActive(true);
            });
            GeneralPopUpData.ButtonData closeButton = new GeneralPopUpData.ButtonData("No", () => {
                inputFieldEmail.MobileInputField.gameObject.SetActive(true);
                inputFieldPassword.MobileInputField.gameObject.SetActive(true);
            });
            GeneralPopUpData data = new GeneralPopUpData("Email verification", $"You have not activated your account via the email, would you like us to send it again? \n {email}", closeButton, funcButton);

            WarningConstructor.OnShow?.Invoke(data);
        } else {
            GeneralPopUpData.ButtonData closeButton = new GeneralPopUpData.ButtonData("Ok", () => {
                inputFieldEmail.MobileInputField.gameObject.SetActive(true);
                inputFieldPassword.MobileInputField.gameObject.SetActive(true);
            });
            GeneralPopUpData data = new GeneralPopUpData("Email verification", $"You have not activated your account via the email \n {email}", closeButton);

            WarningConstructor.OnShow?.Invoke(data);
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

    /// <summary>
    /// Switch sign in to profile
    /// </summary>
    public void SignInToProfile() {
        SignInConstructor.OnHide?.Invoke();
        CreateUsernameConstructor.OnShow?.Invoke();
    }

    /// <summary>
    /// Switch sign in to sign up
    /// </summary>
    public void SignInToSignUp() {
        SignInConstructor.OnHide?.Invoke();
        SignUpConstructor.OnShow?.Invoke();
    }

    /// <summary>
    /// Switch sign in to reset password
    /// </summary>
    public void SignInToResetPassword() {
        SignInConstructor.OnHide?.Invoke();
        ResetPasswordConstructor.OnShow?.Invoke();
    }

    /// <summary>
    /// Switch sign in to welcome
    /// </summary>
    public void SignInToWelcome() {
        SignInConstructor.OnHide?.Invoke();
        WelcomeConstructor.OnShow?.Invoke();
        _accountManager.LogOut();
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