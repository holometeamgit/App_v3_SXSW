using CleverTap;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsCleverTapController : AnalyticsLibraryAbstraction
{
    public static AnalyticsCleverTapController Instance { get; private set; }

    const string AccountID = "65R-W6K-KW6Z";
    const string Token = "360-256";

    bool disableTracking;

    [SerializeField]
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
            //cleverTapUnityComponent = gameObject.AddComponent<CleverTapUnity>();
            if (cleverTapUnityComponent.CLEVERTAP_ACCOUNT_ID != AccountID)
            {
                Debug.LogError("CleverTap Account ID didn't match");
                cleverTapUnityComponent.CLEVERTAP_ACCOUNT_ID = AccountID;
            }
            if (cleverTapUnityComponent.CLEVERTAP_ACCOUNT_TOKEN != Token)
            {
                Debug.LogError("CleverTap Account Token didn't match");
                cleverTapUnityComponent.CLEVERTAP_ACCOUNT_TOKEN = Token;
            }
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
        CleverTapBinding.RecordEvent(eventName);
    }

    public override void SendCustomEvent(string eventName, string dataName, object data)
    {
        var dataContainer = new Dictionary<string, object>();
        dataContainer.Add(dataName, data);
        CleverTapBinding.RecordEvent(eventName, dataContainer);
    }
}
