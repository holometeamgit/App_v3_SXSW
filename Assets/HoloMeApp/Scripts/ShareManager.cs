using UnityEngine;
using NatShare;
using Beem.Firebase.DynamicLink;
using System;
using Firebase.DynamicLinks;

public class ShareManager : MonoBehaviour {

    [SerializeField]
    private ServerURLAPIScriptableObject serverURLAPIScriptableObject;

    private void OnEnable() {
        DynamicLinksCallBacks.onGetShortLink += ShareStream;
        DynamicLinksCallBacks.onShareAppLink += ShareApp;
    }

    private void OnDisable() {
        DynamicLinksCallBacks.onGetShortLink -= ShareStream;
        DynamicLinksCallBacks.onShareAppLink -= ShareApp;
    }

    private void ShareApp() {
        Uri link = new Uri(serverURLAPIScriptableObject.FirebaseDynamicLink + "/" + serverURLAPIScriptableObject.App);
        string msg = "Come to my App: " + link.AbsoluteUri;
        HelperFunctions.DevLog(msg);
        ShareLink(msg);
    }

    private void ShareStream(Uri link, SocialMetaTagParameters socialMetaTagParameters) {
        string msg = socialMetaTagParameters.Title + "\n" + socialMetaTagParameters.Description + "\n" + link.AbsoluteUri;
        ShareLink(msg);
    }

    protected void ShareLink(string msg) {

#if !UNITY_EDITOR
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyShareEventPressed);
        new NativeShare().SetText(msg).Share();
#else
        HelperFunctions.DevLog(msg);
#endif
    }
}
