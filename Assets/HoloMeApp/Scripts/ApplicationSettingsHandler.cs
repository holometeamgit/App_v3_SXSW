using UnityEngine;

public class ApplicationSettingsHandler : MonoBehaviour
{
    const int SLEEP_TIMEOUT = 60;
    const int TARGET_FRAAME_RATE = 300;

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
    }

    public void ToggleSleepTimeout(bool disable) {
        Screen.sleepTimeout = disable? SleepTimeout.NeverSleep : SLEEP_TIMEOUT;
    }
}
