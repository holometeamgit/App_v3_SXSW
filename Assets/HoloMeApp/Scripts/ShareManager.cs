using UnityEngine;
using NatShare;
using Beem.Firebase.DynamicLink;
using System;

public class ShareManager : MonoBehaviour {

    [SerializeField]
    private ServerURLAPIScriptableObject serverURLAPIScriptableObject;

    private const string LINK_DESCRIPTION = "Here's your Beem invite from {0}\nPlease click the link to access their Room\n{1}";

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

    private void ShareStream(Uri link, string source) {
        string msg = string.Format(LINK_DESCRIPTION, source, link.AbsoluteUri);
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
