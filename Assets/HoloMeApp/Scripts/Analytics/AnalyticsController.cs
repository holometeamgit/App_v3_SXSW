﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsController : MonoBehaviour {
    public static AnalyticsController Instance { get; private set; }
    
    Dictionary<string, AnalyticsDwellTracker> dwellTimers = new Dictionary<string, AnalyticsDwellTracker>();

    [SerializeField]
    CleverTapUnity  cleverTapUnity;

    [SerializeField]
    AppsFlyerObjectScript appsFlyerObjectScript;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(Instance);
        } else {
            Debug.LogError($"{nameof(AnalyticsController)} Instance Already Exists!");
            Destroy(Instance);
        }
    }

    public void SendCustomEvent(string eventName) {
        if (string.IsNullOrWhiteSpace(eventName)) {
            Debug.LogError("Custom event name wasn't specified");
            return;
        }

        HelperFunctions.DevLog($"Custom Event Sent {eventName}");
        Analytics.CustomEvent(eventName);
        cleverTapUnity.SendCustomEvent(eventName);
    }

    public void SendCustomEvent(string eventName, string dataName, object data)
    {
        if (string.IsNullOrWhiteSpace(eventName))
        {
            Debug.LogError("Custom event name wasn't specified");
            return;
        }
        var dataContainer = new Dictionary<string, object>();
        dataContainer.Add(dataName, data);

        HelperFunctions.DevLog($"Custom Event Sent {eventName} with data {dataName} {data}");
        Analytics.CustomEvent(eventName, dataContainer);
        cleverTapUnity.SendCustomEvent(eventName, dataContainer);
        appsFlyerObjectScript.SendCustomEvent(eventName, dataName, data);
    }

    public void StartTimer(string timerKey, string timerName) {
        if (dwellTimers.ContainsKey(timerKey)) {
            Debug.LogError("Timer already exists in collection " + timerKey);
            return; //dwellTimers[timerName];
        }
        var dwellTimer = gameObject.AddComponent<AnalyticsDwellTracker>();
        dwellTimer.trackerName = timerName;
        dwellTimer.StartTimer();
        dwellTimers.Add(timerKey, dwellTimer);

        HelperFunctions.DevLog($"Added timer {timerKey}");

        //return dwellTimer;
    }

    public void StopTimer(string timerName) {
        if (!dwellTimers.ContainsKey(timerName)) {
            Debug.LogError("Timer didn't exist in collection " + timerName);
            return;
        }

        var timer = dwellTimers[timerName];
        RemoveTimer(timer, timerName, timer.trackerName, timer.Timer);
    }

    public void StopTimer(string timerName, float customTime) {
        if (!dwellTimers.ContainsKey(timerName)) {
            Debug.LogError("Timer didn't exist in collection " + timerName);
            return;
        }

        var timer = dwellTimers[timerName];
        RemoveTimer(timer, timerName, timer.trackerName, customTime);
    }

    /// <param name="timerDictonaryKey">Required to remove timer</param>
    /// <param name="timerName">Name to be shown in analytics</param>
    /// <param name="elapsedTime">Time specified</param>
    private void RemoveTimer(AnalyticsDwellTracker dwellTimercomponent, string timerDictonaryKey, string timerName, float elapsedTime) {

        var time = new Dictionary<string, object>();
        time.Add(timerName, elapsedTime);
        Analytics.CustomEvent(timerName, time);
        dwellTimers.Remove(timerDictonaryKey);

        HelperFunctions.DevLog($"Dwell timer {timerName} stopped. Time tracked = {elapsedTime}");
        Destroy(dwellTimercomponent);
    }

    public int GetElapsedTime(string timerName) {
        if (!dwellTimers.ContainsKey(timerName)) {
            Debug.LogError("Timer didn't exist in collection " + timerName);
            return 0;
        }
        return dwellTimers[timerName].Timer;
    }

    public void PauseTimer(string timerName) {
        if (!dwellTimers.ContainsKey(timerName)) {
            Debug.LogError("Timer didn't exist in collection " + timerName);
            return;
        }

        dwellTimers[timerName].PauseTimer();
    }

    public void ResumeTimer(string timerName) {
        if (!dwellTimers.ContainsKey(timerName)) {
            Debug.LogError("Timer didn't exist in collection " + timerName);
            return;
        }

        dwellTimers[timerName].ResumeTimer();
    }
}
