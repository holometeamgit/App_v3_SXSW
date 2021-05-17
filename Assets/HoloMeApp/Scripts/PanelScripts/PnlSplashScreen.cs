using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
using Beem.SSO;
using System;

public class PnlSplashScreen : MonoBehaviour
{
    public Action onViewStartHide;
    public Action onViewEnabled;

    [SerializeField] GameObject updateRect;

    [SerializeField] List<GameObject> specificAppleUIGONeedActive;
    [SerializeField] Animator animator;

    [SerializeField] UnityEvent OnLogInEvent;
    [SerializeField] UnityEvent OnAuthorisationErrorEvent;

    public void OnAuthorisation() {
        HideSplashScreen();
        OnLogInEvent.Invoke();
    }

    public void OnAuthorisationErrorInvoke() {
        OnAuthorisationErrorEvent.Invoke();
        HideSplashScreen();
    }

    public void ShowNeedUpdate() {
        updateRect.SetActive(true);
    }

    #region call from animation

    /// <summary>
    /// Disable Splash Screen after end animation
    /// Invoke from animation now
    /// </summary>
    public void DisableSplashScreen() {
        gameObject.SetActive(false);
    }

    #endregion

    private void Awake() {
#if UNITY_IOS
        foreach (var appleUI in specificAppleUIGONeedActive) {
            appleUI.SetActive(true);
        }
#endif
    }

    private void Start() {
        onViewEnabled?.Invoke();
    }

    private void HideSplashScreen() {
        onViewStartHide?.Invoke();
        animator.SetBool("Hide", true);
    }
}
