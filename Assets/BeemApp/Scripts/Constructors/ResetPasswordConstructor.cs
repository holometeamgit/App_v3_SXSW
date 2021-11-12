using System;
/// <summary>
/// Constructor for reset Password
/// </summary>
public class ResetPasswordConstructor : WindowConstructor {
    public static Action<bool> OnActivated = delegate { };

    protected void OnEnable() {
        OnActivated += Activate;
    }

    protected void OnDisable() {
        OnActivated -= Activate;
    }

    protected void Activate(bool status) {
        _window.SetActive(status);
    }
}
