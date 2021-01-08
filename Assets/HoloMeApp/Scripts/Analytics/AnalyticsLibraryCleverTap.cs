using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnalyticsLibrary", menuName = "Data/Analytics/AnalyticsLibraryCleverTap")]
public class AnalyticsLibraryCleverTap : AnalyticsLibrary
{
    [SerializeField]
    CleverTapUnity PrefabCleverTap;

    public override void SendCustomEvent(string eventName, Dictionary<string, object> eventData)
    {
        PrefabCleverTap.SendCustomEvent(eventName, eventData);
        Debug.Log("CLEVER TAP " + PrefabCleverTap == null + PrefabCleverTap.transform.name);
    }
    public override void SendCustomEvent(string eventName)
    {
        PrefabCleverTap.SendCustomEvent(eventName);
        Debug.Log("CLEVER TAP " + PrefabCleverTap.transform.position);
    }
}
