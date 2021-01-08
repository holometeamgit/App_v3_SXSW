using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

[CreateAssetMenu(fileName = "AnalyticsLibrary", menuName = "Data/Analytics/AnalyticsLibraryUnity")]
public class AnalyticsLibraryUnity : AnalyticsLibrary
{
    public override void SendCustomEvent(string eventName)
    {
        Analytics.CustomEvent(eventName);
    }

    public override void SendCustomEvent(string eventName, Dictionary<string, object> eventData)
    {
        Analytics.CustomEvent(eventName, eventData);
    }
}
