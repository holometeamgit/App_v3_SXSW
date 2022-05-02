using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

public class iOSContactsListPlugin : MonoBehaviour {

#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern string _GetAllContacts();
#endif

    public static string GetAllContacts() {
#if UNITY_IOS
        return _GetAllContacts();
#endif
        return "";
    }
}
