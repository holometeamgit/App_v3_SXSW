using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsController : MonoBehaviour {
    public static AnalyticsController Instance { get; private set; }

    Dictionary<string, AnalyticsDwellTracker> dwellTimers = new Dictionary<string, AnalyticsDwellTracker>();

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

        Analytics.CustomEvent(eventName);
    }

    public void StartTimer(string timerKey, string timerName) {
        if (dwellTimers.ContainsKey(timerKey)) {
            Debug.LogError("Timer already exists in collection " + timerKey);
            return; //dwellTimers[timerName];
        }
        var dwellTimer = gameObject.AddComponent<AnalyticsDwellTracker>();
        dwellTimer.name = timerName;
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
        var time = new Dictionary<string, object>();
        time.Add(dwellTimers[timerName].name, timer.Timer);
        Analytics.CustomEvent(timerName, time);
        dwellTimers.Remove(timerName);

        HelperFunctions.DevLog($"Dwell timer {timerName} stopped. Time tracked = {timer.Timer}");
        Destroy(timer);
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
