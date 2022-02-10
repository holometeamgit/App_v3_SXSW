using System;
using UnityEngine;

/// <summary>
/// Constructor for splash screen
/// </summary>
public class SplashScreenConstructor : MonoBehaviour {
    [SerializeField]
    private PnlSplashScreen _pnlSplashScreen;

    public static Action<SplashScreenData> OnShow = delegate { };
    public static Action OnHide = delegate { };

    public static bool IsActive;

    private void OnEnable() {
        OnShow += Show;
        OnHide += Hide;
    }

    private void OnDisable() {
        OnShow -= Show;
        OnHide -= Hide;
    }


    private void Show(SplashScreenData data) {
        IsActive = true;
        _pnlSplashScreen.Show(data);
    }

    private void Hide() {
        IsActive = false;
        _pnlSplashScreen.Hide();
    }

}

