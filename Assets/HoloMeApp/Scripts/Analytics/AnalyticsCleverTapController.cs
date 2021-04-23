using CleverTap;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsCleverTapController : AnalyticsLibraryAbstraction
{
    public static AnalyticsCleverTapController Instance { get; private set; }

    const string AccountID = "65R-W6K-KW6Z";
    const string Token = "360-256";

    //bool disableTracking;

    [SerializeField]
    CleverTapUnity cleverTapUnityComponent;

    [SerializeField]
    UserWebManager userWebManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //#if DEV
            //            disableTracking = true;
            //#endif
            //cleverTapUnityComponent = gameObject.AddComponent<CleverTapUnity>();

            cleverTapUnityComponent = HelperFunctions.GetTypeIfNull<CleverTapUnity>(cleverTapUnityComponent);
            userWebManager = HelperFunctions.GetTypeIfNull<UserWebManager>(userWebManager);

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

#if UNITY_IOS && !UNITY_EDITOR
            CleverTapBinding.RegisterPush();
#endif

            DontDestroyOnLoad(Instance);
        }
        else
        {
            Debug.LogError($"{nameof(AnalyticsController)} Instance Already Exists!");
            Destroy(Instance);
        }
    }

    void UpdateUserProfile()
    {
        //Debug.Log("USER LONG INFO PASSED CT 1");
        //if (userWebManager == null)
        //{
        //    //Debug.Log(" USER WEB MANAGER WAS NULL");
        //}
        //else
        //{
        //    Debug.Log("NAME = " + userWebManager.GetUsername());
        //}

        Dictionary<string, string> loginDetails = new Dictionary<string, string>();
        loginDetails.Add("Email", userWebManager.GetEmail());
        loginDetails.Add("Name", userWebManager.GetUsername());
        loginDetails.Add("Username", userWebManager.GetUsername());
        loginDetails.Add("UserID", userWebManager.GetUserID().ToString());
        loginDetails.Add("Identity", userWebManager.GetUserID().ToString());
        CleverTapBinding.OnUserLogin(loginDetails);

#if UNITY_ANDROID
        CleverTapBinding.CreateNotificationChannel("Default", "Default", "Default", 1, true);
#endif

        //Debug.Log("USER LONG INFO PASSED CT2");
    }

    public override void SendCustomEvent(string eventName)
    {
        if (Application.isEditor) //Stops android exception
            return;

        CleverTapBinding.RecordEvent(eventName);
    }

    //public override void SendCustomEvent(string eventName, string dataName, object data)
    //{
    //    var dataContainer = new Dictionary<string, object>();
    //    dataContainer.Add(dataName, data);
    //    CleverTapBinding.RecordEvent(eventName, dataContainer);
    //}

    public override void SendCustomEvent(string eventName, Dictionary<string, string> data)
    {
        //Without the return below the app won't login in the editor test on device for auto login tracking
        if (Application.isEditor) //Stops android exception
            return;

        if (eventName.Contains(AnalyticKeys.KeyUserLogin))//Contains in case of dev_ prefix
        {
            UpdateUserProfile();
        }

        data?.Add(AnalyticParameters.ParamUserEmail, userWebManager.GetEmail() ?? "Not specified"); //Append email for CT only
        CleverTapBinding.RecordEvent(eventName, ConvertToStringObjectDictionary(data));
    }

    public void SendChargeEvent(Dictionary<string, object> chargeDetails, List<Dictionary<string, object>> items)
    {
        if (Application.isEditor) //Stops android exception
            return;

        chargeDetails?.Add(AnalyticParameters.ParamUserEmail, userWebManager.GetEmail() ?? "Not specified"); //Append email for CT only
        CleverTapBinding.RecordChargedEventWithDetailsAndItems(chargeDetails, items);
    }
}
