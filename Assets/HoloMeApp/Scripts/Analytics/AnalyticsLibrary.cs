using System.Collections.Generic;
using UnityEngine;

public abstract class AnalyticsLibrary : ScriptableObject
{
    public abstract void SendCustomEvent(string eventName);
    public abstract void SendCustomEvent(string eventName, Dictionary<string, object> eventData);
}
