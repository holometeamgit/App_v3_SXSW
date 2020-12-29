using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PnlSplashScreen : MonoBehaviour
{
    [SerializeField] AccountManager accountManager;
    [SerializeField] FacebookAccountManager facebookAccountManager;
    [SerializeField] AppleAccountManager appleAccountManager;
    [SerializeField] EmailAccountManager emailAccountManager;
    [SerializeField] GameObject updateRect;
    [SerializeField] VersionChecker versionChecker;

    public UnityEvent OnLogInEvent;
    public UnityEvent OnAuthorisationErrorEvent;

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
    }

    void Start()
    {
        versionChecker.RequestVersion();
    }

    private void ShowNeedUpdate() {
        updateRect.SetActive(true);
    }

    private void TryLogin() {
        //try get new access token from server via refresh access token. If it doesn’t work, then try to get a new refresh token
//        Debug.Log("Last Login Type " + (LogInType)PlayerPrefs.GetInt(PlayerPrefsKeys.LastTypeLoginPPKey));
        accountManager.LogIn(LogInCallBack, TryGetNewRefreshTokenAndLogIn);
    }

    private void TryGetNewRefreshTokenAndLogIn(long code, string body) {
        Debug.Log("TryGetNewRefreshTokenAndLogIn");
        Debug.Log(code + " : " + body);

        AuthorisationErrorCallBack(code, body);

        /*
        //if the user has not logged in then send to the registration menu
        if (!PlayerPrefs.HasKey(PlayerPrefsKeys.LastTypeLoginPPKey)) {
            AuthorisationErrorCallBack(0, "Doesn't contain Authorisation key");
            return;
        }

        LogInType logInType = (LogInType)PlayerPrefs.GetInt(PlayerPrefsKeys.LastTypeLoginPPKey);
        switch (logInType) {
        case LogInType.None:
            //if the user has not logged in or logged out then send to the registration menu //TODO add to log out set LastTypeLoginPPKey to None
            AuthorisationErrorCallBack(0, "Not authorized");
            break;
        case LogInType.Apple:
            appleAccountManager.AttemptQuickLogin();
            break;
        case LogInType.Email:
            AuthorisationErrorCallBack(0, "the user needs to enter log in data himself");  //because we do not store the email and password of the user//TODO maybe try later save in android and ios

            break;
        case LogInType.Facebook:
            facebookAccountManager.LogInWithSavedAccessToken(LogInCallBack, AuthorisationErrorCallBack);
            break;
        case LogInType.Google:
            AuthorisationErrorCallBack(0, "it has not yet been implemented");  //TODO update after it will add to the server
            break;
        }*/
    }

    private void AuthorisationErrorCallBack(long code, string body) {
        Debug.Log("AuthorisationErrorCallBack " + code + " " + body);

        Invoke("AuthorisationErrorInvoke", 2f);
    }

    public void AuthorisationErrorInvoke() {
        OnAuthorisationErrorEvent.Invoke();
    }

    private void LogInCallBack(long code, string body) {
        Invoke("LogInIvoke", 2f);
    }

    private void LogInIvoke() {
//        Debug.Log("LogInCallBack");

        OnLogInEvent.Invoke();
    }
}
