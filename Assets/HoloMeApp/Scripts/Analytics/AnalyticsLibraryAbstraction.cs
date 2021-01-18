using System.Collections.Generic;
using UnityEngine;

public abstract class AnalyticsLibraryAbstraction : MonoBehaviour
{
    public abstract void SendCustomEvent(string eventName);
    public abstract void SendCustomEvent(string eventName, string dataName, object data);
}
