using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
using Beem.SSO;

public class PnlSplashScreen : MonoBehaviour
{
    [SerializeField] AccountManager accountManager;
    [SerializeField] GameObject updateRect;
    [SerializeField] VersionChecker versionChecker;

    [SerializeField] List<GameObject> specificAppleUIGONeedActive;
    [SerializeField] Animator animator;

    [SerializeField] UnityEvent OnLogInEvent;
    [SerializeField] UnityEvent OnAuthorisationErrorEvent;

    private const int HIDE_SPLASH_SCREEN_TIME = 6000;

    public void OpenStore() {
#if UNITY_IOS
        Application.OpenURL("https://apps.apple.com/au/app/beem-me/id1532446771");
#elif UNITY_ANDROID
    Application.OpenURL("https://play.google.com/store/apps/details?id=com.HoloMe.Beem");
#endif
    }

    public void LogInIvoke() {
        OnLogInEvent.Invoke();
        HideSplashScreen();
    }

    public void AuthorisationErrorInvoke() {
        OnAuthorisationErrorEvent.Invoke();
        HideSplashScreen();
    }

    public void DisableSplashScreen() {
        gameObject.SetActive(false);
    }

    private void Awake() {
        versionChecker.OnCanUse += TryLogin;
        versionChecker.OnNeedUpdateApp += ShowNeedUpdate;
#if UNITY_IOS
        foreach (var appleUI in specificAppleUIGONeedActive) {
            appleUI.SetActive(true);
        }
#endif
    }

    void Start()
    {
        versionChecker.RequestVersion();
    }

    private void ShowNeedUpdate() {
        updateRect.SetActive(true);
    }

    private void TryLogin() {
        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task.Delay(HIDE_SPLASH_SCREEN_TIME).ContinueWith((_) => HideSplashScreen(), taskScheduler);
        accountManager.QuickLogInWithDelay();
    }

    private void ErrorLogInCallBack(string body) {
//        HelperFunctions.DevLogError("ErrorLogInCallBack: " + " : "+ body);
        AuthorisationErrorInvoke();
    }

    private void FirebaseErrorLogIn(string msg) {
        ErrorLogInCallBack("FirebaseErrorLogIn " + msg);
        accountManager.LogOut();
    }

    private void HideSplashScreen() {
        animator.SetBool("Hide", true);
    }

    private void OnEnable() {
        CallBacks.onSignInSuccess += LogInIvoke;
        CallBacks.onFail += FirebaseErrorLogIn;
        CallBacks.onNeedVerification += FirebaseErrorLogIn;
    }

    private void OnDisable() {
        CallBacks.onSignInSuccess -= LogInIvoke;
        CallBacks.onFail -= FirebaseErrorLogIn;
        CallBacks.onNeedVerification -= FirebaseErrorLogIn;
    }

}
