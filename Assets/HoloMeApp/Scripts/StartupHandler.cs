using UnityEngine;

public class StartupHandler : MonoBehaviour
{
    const int SLEEP_TIMEOUT = 60;
    const int TARGET_FRAAME_RATE = 300;

    private void Awake()
    {
        Application.targetFrameRate = TARGET_FRAAME_RATE;
        Screen.sleepTimeout = SLEEP_TIMEOUT;
    }
}
