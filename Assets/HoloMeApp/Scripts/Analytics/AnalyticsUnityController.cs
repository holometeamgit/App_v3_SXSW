using System.Collections.Generic;
using UnityEngine.Analytics;

public class AnalyticsUnityController : AnalyticsLibraryAbstraction
{
    public override void SendCustomEvent(string eventName)
    {
        Analytics.CustomEvent(eventName);
    }

    public override void SendCustomEvent(string eventName, Dictionary<string, string> data)
    {
        Analytics.CustomEvent(eventName, ConvertToStringObjectDictionary(data));
    }
}
