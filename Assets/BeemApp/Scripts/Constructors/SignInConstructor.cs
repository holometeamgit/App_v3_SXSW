
using System;
/// <summary>
/// Constructor for Sign In Window
/// </summary>
public class SignInConstructor : WindowConstructor {
    public static Action OnShow = delegate { };
    public static Action OnHide = delegate { };

    protected void OnEnable() {
        OnShow += Show;
        OnHide += Hide;
    }

    protected void OnDisable() {
        OnShow -= Show;
        OnHide -= Hide;
    }

    protected void Show() {
        _window.SetActive(true);
    }

    protected void Hide() {
        _window.SetActive(false);
    }
}
