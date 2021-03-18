﻿using UnityEngine;
using NatShare;
using Beem.Firebase.DynamicLink;
using System;

public class ShareManager : MonoBehaviour {

    [SerializeField]  
    private ServerURLAPIScriptableObject serverURLAPIScriptableObject;

    private void OnEnable()
    {
        DynamicLinksCallBacks.onGetShortLink += ShareRoom;
        DynamicLinksCallBacks.onShareLink += ShareApp;
    }

    private void OnDisable()
    {
        DynamicLinksCallBacks.onGetShortLink -= ShareRoom;
        DynamicLinksCallBacks.onShareLink -= ShareApp;
    }

    private void ShareApp() {
        Uri link = new Uri(serverURLAPIScriptableObject.DevFirebaseDynamicLink + "/" + serverURLAPIScriptableObject.App);
        string msg = "Come to my App: " + link.AbsoluteUri;
        HelperFunctions.DevLog(msg);
        ShareLink(link);
    }

    private void ShareRoom(Uri link) {
        string msg = "Come to my room: " + link.AbsoluteUri;
        HelperFunctions.DevLog(msg);
        ShareLink(link);
    }

    protected void ShareLink(Uri link)
    {
#if !UNITY_EDITOR
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyShareEventPressed);
        new NativeShare().SetText(link.AbsoluteUri).Share();
#endif
    }
}
