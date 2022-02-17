using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
using Beem.SSO;
using System;

public class PnlSplashScreen : MonoBehaviour {

    [SerializeField]
    private GameObject updateRect;

    [SerializeField]
    private Animator animator;

    /// <summary>
    /// Show Splash Screen window
    /// </summary>
    /// <param name="needUpdate"></param>
    public void Show(bool needUpdate) {
        gameObject.SetActive(true);
        updateRect.SetActive(needUpdate);
    }

    /// <summary>
    /// Hide SpashScreen Window
    /// </summary>
    public void Hide() {
        HideSplashScreen();
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

    private void HideSplashScreen() {
        animator.SetBool("Hide", true);
    }
}
