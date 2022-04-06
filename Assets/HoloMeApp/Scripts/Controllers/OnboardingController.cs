using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnboardingController
{
    private UserWebManager _userWebManager;
    private const string SUFFIX = "NewUserCheck";

    public OnboardingController(UserWebManager userWebManager) {
        _userWebManager = userWebManager;
    }

    public bool IsNewUser() {
#if !UNITY_EDITOR
        if (PlayerPrefs.HasKey(_userWebManager.GetUsername() + SUFFIX))
            return false;
#endif

        PlayerPrefs.SetString(_userWebManager.GetUsername() + SUFFIX, _userWebManager.GetUsername());
        PlayerPrefs.Save();
        return true;
    }

    public void ActivateMainMenu() {
        MenuConstructor.OnActivated?.Invoke(true);
    }
}
