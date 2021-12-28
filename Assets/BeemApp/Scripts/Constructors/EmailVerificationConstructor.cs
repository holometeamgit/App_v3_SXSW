
using System;
/// <summary>
/// Constructor for Email Verification
/// </summary>
public class EmailVerificationConstructor : WindowConstructor {

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
