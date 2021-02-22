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
        Dictionary<string, object> convertedData = new Dictionary<string, object>();
        foreach (KeyValuePair<string, string> eventData in data)
        {
            convertedData.Add(eventData.Key, eventData.Value);
        }
        Analytics.CustomEvent(eventName, convertedData);
    }
}
