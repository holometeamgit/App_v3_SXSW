using System.Collections.Generic;
using UnityEngine;

public class AnalyticsCleverTapController : AnalyticsLibraryAbstraction
{
    public static AnalyticsCleverTapController Instance { get; private set; }

    const string AccountID = "65R-W6K-KW6Z";
    const string Token = "360-256";

    bool disableTracking;
    CleverTapUnity cleverTapUnityComponent;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
#if DEV
            disableTracking = true;
#endif
            cleverTapUnityComponent = gameObject.AddComponent<CleverTapUnity>();
            cleverTapUnityComponent.CLEVERTAP_ACCOUNT_ID = AccountID;
            cleverTapUnityComponent.CLEVERTAP_ACCOUNT_TOKEN = Token;
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
        cleverTapUnityComponent.SendCustomEvent(eventName);
    }
      
    public override void SendCustomEvent(string eventName, string dataName, object data)
    {
        var dataContainer = new Dictionary<string, object>();
        dataContainer.Add(dataName, data);
        cleverTapUnityComponent.SendCustomEvent(eventName, dataContainer);
    }
}
