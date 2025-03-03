﻿using UnityEngine;

public class ApplicationSettingsHandler : MonoBehaviour
{
    public static int TARGET_FRAAME_RATE = 300;
    const int SLEEP_TIMEOUT = 60;

    public static ApplicationSettingsHandler Instance { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Debug.LogError($"A second instance of {nameof(ApplicationSettingsHandler)}");
            Destroy(this);
        }

        Application.targetFrameRate = TARGET_FRAAME_RATE;
        Screen.sleepTimeout = SLEEP_TIMEOUT;
        TouchScreenKeyboard.hideInput = true;
    }

    public void ToggleSleepTimeout(bool disable) {
        HelperFunctions.DevLog("Sleep Timeout Disabled = "+ disable);
        Screen.sleepTimeout = disable? SleepTimeout.NeverSleep : SLEEP_TIMEOUT;
    }
}
