using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Beem.SSO;
using System.Threading.Tasks;
//TODO all this class temp
public class PnlWelcomeV4 : MonoBehaviour {
    [Serializable]
    public class Providers {
        public List<string> providers;

        public Providers() {
            providers = new List<string>();
        }
    }

    [SerializeField]
    WebRequestHandler webRequestHandler;
    [SerializeField]
    GameObject LogInFBGO;
    [SerializeField]
    GameObject LogInAppleGO;
    [SerializeField]
    GameObject LogInGoogleGO;
    [SerializeField]
    GameObject LogInBackground;
    [SerializeField]
    PnlGenericError pnlGenericError;

    [SerializeField]
    Switcher switcherToProfile;
    [SerializeField]
    Switcher switcherToLigIn;

    private const int TIME_FOR_AUTOHIDINGBG = 5000;

    private void Awake() {
    }

    private void EnableFB(long key, string body) {
        try {
            Providers providers = JsonUtility.FromJson<Providers>(body);
            if (providers != null) {
//                LogInFBGO.SetActive(providers.providers.Contains("fb"));
#if UNITY_IOS
                LogInAppleGO.SetActive(providers.providers.Contains("apple"));
#endif
                LogInGoogleGO.SetActive(providers.providers.Contains("google"));
            }
        } catch (Exception e) { }
    }

    private void ShowBackground() {
        LogInBackground.SetActive(true);
    }
    #region autohide
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
        if (LogInBackground.activeSelf) {
            HideBackground();
        }
    }
    #endregion

    private void HideBackground() {
        LogInBackground.SetActive(false);
    }

    private void HideBackground(string reason) {
        LogInBackground.SetActive(false);
    }

    private void SwitchToProfile() {
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
        CallBacks.onFail += AutoHideLoadingBackground;
        CallBacks.onFirebaseSignInSuccess += AutoHideLoadingBackground;

        CallBacks.onSignInFacebook += ShowBackground;
        CallBacks.onSignInApple += ShowBackground;
        CallBacks.onSignInGoogle += ShowBackground;
        CallBacks.onSignInSuccess += HideBackground;
        CallBacks.onFail += HideBackground;
        CallBacks.onNeedVerification += HideBackground;
        webRequestHandler.GetRequest(webRequestHandler.ServerProvidersAPI, EnableFB, (key, body) => { }, null);

        //TODO move to place when user loggined and take info from UserWebManager
        //AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyUserLogin, AnalyticParameters.ParamUserType , accountManager.GetAccountType() == AccountManager.AccountType.Subscriber ? AnalyticParameters.ParamViewer:AnalyticParameters.ParamBroadcaster ); // Using keys in case enum changes names in future
        CallBacks.onSignInSuccess += () => AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyUserLogin);
    }

    private void OnDisable() {
        CallBacks.onSignInSuccess -= SwitchToProfile;
        CallBacks.onFail -= ShowSpecialErrorFacebookFirebaseMsg;
        CallBacks.onFail -= AutoHideLoadingBackground;
        CallBacks.onFirebaseSignInSuccess -= AutoHideLoadingBackground;

        CallBacks.onSignInFacebook -= ShowBackground;
        CallBacks.onSignInApple -= ShowBackground;
        CallBacks.onSignInGoogle -= ShowBackground;
        CallBacks.onSignInSuccess -= HideBackground;
        CallBacks.onFail -= HideBackground;
        CallBacks.onNeedVerification -= HideBackground;
    }
}
