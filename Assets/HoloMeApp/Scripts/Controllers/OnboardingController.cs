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

    /// <summary>
    /// check is new user
    /// </summary>
    public bool IsNewUser() {
        if (PlayerPrefs.HasKey(_userWebManager.GetUsername() + SUFFIX))
            return false;

        PlayerPrefs.SetString(_userWebManager.GetUsername() + SUFFIX, _userWebManager.GetUsername());
        PlayerPrefs.Save();
        return true;
    }

    public void ActivateMainMenu() {
        MenuConstructor.OnActivated?.Invoke(true);
    }
}
