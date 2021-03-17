using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StreamBillingJsonData
{
    //"google_play" or "app_store"
    public string store;
    public Bill bill;

    public StreamBillingJsonData() {
#if UNITY_IOS
        store = "app_store";
#elif UNITY_ANDROID
        store = "google_play";
#endif
        bill = new Bill();
    }

    [Serializable]
    public class Bill {
        public string hash;
    }
}
