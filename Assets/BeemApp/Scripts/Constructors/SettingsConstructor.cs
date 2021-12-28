using System;

/// <summary>
/// Constructor for Settings window
/// </summary>
public class SettingsConstructor : WindowConstructor {
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
