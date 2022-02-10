using UnityEngine;

public class PnlSplashScreen : MonoBehaviour {

    [SerializeField]
    private GameObject updateRect;

    [SerializeField]
    private Animator animator;

    /// <summary>
    /// Show Splash Screen window
    /// </summary>
    /// <param name="needUpdate"></param>
    public void Show(SplashScreenData data) {
        gameObject.SetActive(true);
        updateRect.SetActive(!data.IsLastVersion);
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
