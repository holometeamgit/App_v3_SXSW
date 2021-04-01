using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the universal Analytics class used to call same tracking calls across all libraries
/// </summary>
public class AnalyticsController : MonoBehaviour
{
    public static AnalyticsController Instance { get; private set; }

    Dictionary<string, AnalyticsDwellTracker> dwellTimers = new Dictionary<string, AnalyticsDwellTracker>();

    [SerializeField]
    bool enableDebugTracking;
    bool disableTracking;

    [SerializeField]
    AnalyticsLibraryAbstraction[] analyticsLibraryAbstractions;

    [SerializeField]
    UserWebManager userWebManager;

    public string GetUserID
    {
        get
        {
            return userWebManager.GetUserID().ToString();
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
#if DEV
            if (!enableDebugTracking)
                disableTracking = true;
#endif
            DontDestroyOnLoad(Instance);

            //CallBacks.onSignInSuccess += () => Instance.SendCustomEvent(AnalyticKeys.KeyUserLogin);
            userWebManager.OnLoadUserDataAfterLogIn += () => Instance.SendCustomEvent(AnalyticKeys.KeyUserLogin);
        }
        else
        {
            Debug.LogError($"{nameof(AnalyticsController)} Instance Already Exists!");
            Destroy(Instance);
        }
    }

    private void AppendDevString(ref string eventName)
    {
#if DEV
        eventName = "dev_" + eventName;
#endif
    }

    public void SendCustomEvent(string eventName)
    {
        if (disableTracking)
            return;

        if (string.IsNullOrWhiteSpace(eventName))
        {
            Debug.LogError("Custom event name wasn't specified");
            return;
        }

        //AppendDevString(ref eventName);

        HelperFunctions.DevLog($"Custom Event Sent {eventName}");

        //foreach (var analyticsController in analyticsLibraryAbstractions)
        //{
        //    analyticsController.SendCustomEvent(eventName);
        //}

        SendCustomEvent(eventName, new Dictionary<string,string>());
    }

    public void SendCustomEvent(string eventName, string dataName, object data)
    {
        if (disableTracking)
            return;

        if (string.IsNullOrWhiteSpace(eventName))
        {
            Debug.LogError("Custom event name wasn't specified");
            return;
        }

        //HelperFunctions.DevLog($"Custom Event Sent {eventName} with data {dataName} {data}");

        Dictionary<string, string> dataDictionary = new Dictionary<string, string>() { { dataName, (string)data } };

        //foreach (var analyticsController in analyticsLibraryAbstractions)
        //{
        //    //analyticsController.SendCustomEvent(eventName, dataName, data);
        //}

        SendCustomEvent(eventName, dataDictionary);
    }

    public void SendCustomEvent(string eventName, Dictionary<string, string> data)
    {
        if (disableTracking)
            return;

        if (string.IsNullOrWhiteSpace(eventName))
        {
            Debug.LogError("Custom event name wasn't specified");
            return;
        }

        AppendDevString(ref eventName);

        HelperFunctions.DevLog($"Custom Event Sent {eventName} with data {data}");

        if (userWebManager != null && userWebManager.GetUserID() != -1)
        {
            data.Add(AnalyticParameters.ParamUserID, userWebManager.GetUserID().ToString()); //Add user ID to tracking variable
            data.Add(AnalyticParameters.ParamUserType, userWebManager.CanGoLive() ? AnalyticParameters.ParamBroadcaster : AnalyticParameters.ParamViewer);
        }
        else
        {
            Debug.LogError(nameof(UserWebManager) + " was null");
        }

        foreach (var analyticsController in analyticsLibraryAbstractions)
        {
            analyticsController.SendCustomEvent(eventName, data);
        }
    }

    public void StartTimer(string timerKey, string timerName)
    {
        if (disableTracking)
            return;

        if (dwellTimers.ContainsKey(timerKey))
        {
            Debug.LogError("Timer already exists in collection " + timerKey);
            return; //dwellTimers[timerName];
        }
        var dwellTimer = gameObject.AddComponent<AnalyticsDwellTracker>();
        dwellTimer.trackerName = timerName;
        dwellTimer.StartTimer();
        dwellTimers.Add(timerKey, dwellTimer);

        HelperFunctions.DevLog($"Added timer {timerKey} with name {timerName}");

        //return dwellTimer;
    }

    public void StopTimer(string timerName,Dictionary<string, string> additonalData = null)
    {
        if (disableTracking)
            return;

        if (!dwellTimers.ContainsKey(timerName))
        {
            Debug.LogError("Timer didn't exist in collection " + timerName);
            return;
        }

        var timer = dwellTimers[timerName];
        RemoveTimer(timer, timerName, timer.trackerName, timer.Timer, additonalData);
    }

    public void StopTimer(string timerName, float customTime, Dictionary<string, string> additonalData = null)
    {
        if (disableTracking)
            return;

        if (!dwellTimers.ContainsKey(timerName))
        {
            Debug.LogError("Timer didn't exist in collection " + timerName);
            return;
        }

        var timer = dwellTimers[timerName];
        RemoveTimer(timer, timerName, timer.trackerName, customTime, additonalData);
    }

    /// <param name="timerDictonaryKey">Required to remove timer</param>
    /// <param name="timerName">Name to be shown in analytics</param>
    /// <param name="elapsedTime">Time specified</param>
    private void RemoveTimer(AnalyticsDwellTracker dwellTimercomponent, string timerDictonaryKey, string timerName, float elapsedTime, Dictionary<string, string> dataDictionary)
    {
        if (disableTracking)
            return;

        AppendDevString(ref timerName);

        if(dataDictionary == null)
            dataDictionary = new Dictionary<string, string>();
        dataDictionary.Add("Time", elapsedTime.ToString());
        dataDictionary.Add(AnalyticParameters.ParamUserID, userWebManager.GetUserID().ToString());

        foreach (var analyticsController in analyticsLibraryAbstractions)
        {
            analyticsController.SendCustomEvent(timerName, dataDictionary);
        }
        dwellTimers.Remove(timerDictonaryKey);

        HelperFunctions.DevLog($"Dwell timer {timerName} stopped. Time tracked = {elapsedTime}");
        Destroy(dwellTimercomponent);
    }

    public int GetElapsedTime(string timerName)
    {
        if (disableTracking)
            return 0;

        if (!dwellTimers.ContainsKey(timerName))
        {
            Debug.LogError("Timer didn't exist in collection " + timerName);
            return 0;
        }
        return dwellTimers[timerName].Timer;
    }

    public void PauseTimer(string timerName)
    {
        if (disableTracking)
            return;

        if (!dwellTimers.ContainsKey(timerName))
        {
            Debug.LogError("Timer didn't exist in collection " + timerName);
            return;
        }

        dwellTimers[timerName].PauseTimer();
    }

    public void ResumeTimer(string timerName)
    {
        if (disableTracking)
            return;

        if (!dwellTimers.ContainsKey(timerName))
        {
            Debug.LogError("Timer didn't exist in collection " + timerName);
            return;
        }

        dwellTimers[timerName].ResumeTimer();
    }
}
