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

    [SerializeField] UnityEvent OnLogInEvent;
    [SerializeField] UnityEvent OnAuthorisationErrorEvent;

    private const int HIDE_SPLASH_SCREEN_TIME = 1000;

    public void OpenStore() {
#if UNITY_IOS
        Application.OpenURL("https://apps.apple.com/au/app/beem-me/id1532446771");
#elif UNITY_ANDROID
    Application.OpenURL("https://play.google.com/store/apps/details?id=com.HoloMe.Beem");
#endif
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
//        Debug.Log("LogInType " + accountManager.GetLogInType());

        accountManager.QuickLogIn(LogInCallBack, ErrorLogInCallBack);
    }

    private void LogInCallBack(long code, string body) {
//        Debug.Log("LogInCallBack " + body);
        var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task.Delay(HIDE_SPLASH_SCREEN_TIME).ContinueWith(_ => LogInIvoke(), taskScheduler);
    }

    private void LogInIvoke() {
        OnLogInEvent.Invoke();
    }

    private void ErrorLogInCallBack(long code, string body) {
        Debug.Log("ErrorLogInCallBack " + code + " : "+ body);
        var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task.Delay(HIDE_SPLASH_SCREEN_TIME).ContinueWith(_ => AuthorisationErrorInvoke(), taskScheduler);
    }

    public void AuthorisationErrorInvoke() {
        OnAuthorisationErrorEvent.Invoke();
    }

    private void FirebaseErrorLogIn(string msg) {
        ErrorLogInCallBack(500, "Can't connect to server");
        accountManager.LogOut();
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
