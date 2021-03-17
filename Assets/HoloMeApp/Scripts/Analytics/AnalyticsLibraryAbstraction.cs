using System.Collections.Generic;
using UnityEngine;

public abstract class AnalyticsLibraryAbstraction : MonoBehaviour
{
    public abstract void SendCustomEvent(string eventName);
    //public abstract void SendCustomEvent(string eventName, string dataName, object data);
    public abstract void SendCustomEvent(string eventName, Dictionary<string, string> data);

    protected Dictionary<string, object> ConvertToStringObjectDictionary(Dictionary<string, string> data)
    {
        Dictionary<string, object> convertedData = new Dictionary<string, object>();
        foreach (KeyValuePair<string, string> eventData in data)
        {
            convertedData.Add(eventData.Key, eventData.Value);
        }
        return convertedData;
    }
}
