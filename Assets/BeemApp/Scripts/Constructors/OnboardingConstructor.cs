
using System;
using UnityEngine;
/// <summary>
/// Constructor for Onboarding In Window
/// </summary>
public class OnboardingConstructor : WindowConstructor {
    public static Action<bool> OnActivated = delegate { };

    [SerializeField]
    private UserWebManager _userWebManager;
    private OnboardingController _onboardingController;

    private void Awake() {
        _onboardingController = new OnboardingController(_userWebManager);
    }

    protected void OnEnable() {
        OnActivated += Activate;
    }

    protected void OnDisable() {
        OnActivated -= Activate;
    }

    protected void Activate(bool status) {
        if (_onboardingController.IsNewUser()) {
            _window.SetActive(status);
        } else {
            _onboardingController.ActivateMainMenu();
        }
    }
}
