using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AnalyticsLibrary", menuName = "Data/Analytics/AnalyticsLibraryAppsFlyer")]
public class AnalyticsLibraryAppsFlyer : AnalyticsLibrary
{
    [SerializeField]
    AppsFlyerObjectScript appsFlyerObjectScript;
    public override void SendCustomEvent(string eventName)
    {
     
    }

    public override void SendCustomEvent(string eventName, Dictionary<string, object> eventData)
    {
        var first = eventData.First();
        appsFlyerObjectScript.SendCustomEvent(eventName, first.Key, first.Value);
    }
}
