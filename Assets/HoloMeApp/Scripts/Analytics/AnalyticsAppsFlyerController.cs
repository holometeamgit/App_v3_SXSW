using AppsFlyerSDK;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is required to hold Ids and in order to make non shared analytics calls unique to AF
/// </summary>
public class AnalyticsAppsFlyerController : AnalyticsLibraryAbstraction
{
    public static AnalyticsAppsFlyerController Instance { get; private set; }

    const string DevKey = "ySBPhh8s4HtZcUiz2QqfVb";
    const string AppID = "1532446771";

    bool disableTracking;

    [SerializeField]
    AppsFlyerObjectScript appsFlyerObjectComponent;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
#if DEV
            disableTracking = true;
#endif
            //appsFlyerObjectComponent = gameObject.AddComponent<AppsFlyerObjectScript>();

            if (appsFlyerObjectComponent.devKey != DevKey)
            {
                Debug.LogError("AppsFlyer Account DevKey didn't match");
                appsFlyerObjectComponent.devKey = DevKey;
            }
            if (appsFlyerObjectComponent.appID != AppID)
            {
                Debug.LogError("AppsFlyer AppID didn't match");
                appsFlyerObjectComponent.appID = AppID;
            }
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Debug.LogError($"{nameof(AnalyticsController)} Instance Already Exists!");
            Destroy(Instance);
        }

    }

    public override void SendCustomEvent(string eventName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Custom function made to fit universal calls
    /// </summary>
    public override void SendCustomEvent(string eventName, string dataName, object data)
    {
        if (data is string)
        {
            HelperFunctions.DevLog("AppsFlyer SendCustomEvent data = " + (string)data);
            Dictionary<string, string> dataToSend = new Dictionary<string, string>();
            dataToSend.Add(dataName, (string)data);

            try
            {
                AppsFlyer.sendEvent(eventName, dataToSend);
            }
            catch (Exception e)
            {
                Debug.LogError("AppsFlyer Event record failed: " + e.ToString());
            }
        }
    }
}
