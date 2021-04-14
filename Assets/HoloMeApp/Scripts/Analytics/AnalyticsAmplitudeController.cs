using System.Collections.Generic;
using UnityEngine;

public class AnalyticsAmplitudeController : AnalyticsLibraryAbstraction
{
    public static AnalyticsAmplitudeController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Amplitude amplitude = Amplitude.getInstance();
            amplitude.setServerUrl("https://api2.amplitude.com");
            amplitude.logging = true;
            amplitude.trackSessionEvents(true);
            amplitude.useAdvertisingIdForDeviceId();
            amplitude.init("22e1d4d5f9f3adcfca153c32fc4d10ae");

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
        Amplitude.Instance.logEvent(eventName);
    }

    public override void SendCustomEvent(string eventName, Dictionary<string, string> data)
    {
        Amplitude.Instance.logEvent(eventName, ConvertToStringObjectDictionary(data));
    }

    public void onApplicationQuit()
    {
        Amplitude.Instance.logEvent("session over");
    }
}
