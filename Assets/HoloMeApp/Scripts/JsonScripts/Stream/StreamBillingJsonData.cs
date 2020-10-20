using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StreamBillingJsonData
{
    public string store;
    public Bill bill;

    public StreamBillingJsonData() {
        bill = new Bill();
    }

    [Serializable]
    public class Bill {
        public string hash;
    }
}
