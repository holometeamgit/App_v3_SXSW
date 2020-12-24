﻿using UnityEngine;

public class StartupHandler : MonoBehaviour
{
    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 300;
        //TouchScreenKeyboard.hideInput = true;
        Screen.sleepTimeout = 60;
    }
}
