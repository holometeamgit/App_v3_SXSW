
using System;
using UnityEngine;
using Zenject;
/// <summary>
/// Constructor for Onboarding In Window
/// </summary>
public class OnboardingConstructor : WindowConstructor {
    public static Action<bool> OnActivated = delegate { };


    private UserWebManager _userWebManager;
    private OnboardingController _onboardingController;

    [Inject]
    public void Construct(UserWebManager userWebManager) {
        _userWebManager = userWebManager;
    }

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
        if (!status) {
            _window.SetActive(status);
            return;
        }

        if (_onboardingController.IsNewUser()) {
            _window.SetActive(status);
        } else {
            _onboardingController.ActivateMainMenu();
        }
    }
}
