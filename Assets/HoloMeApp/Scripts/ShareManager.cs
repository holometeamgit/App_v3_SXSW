using UnityEngine;
using NatShare;
using Beem.Firebase.DynamicLink;
using System;
using Firebase.DynamicLinks;

public class ShareManager : MonoBehaviour {

    private void OnEnable() {
        DynamicLinksCallBacks.onShareSocialLink += ShareSocialLink;
        DynamicLinksCallBacks.onShareLink += ShareLink;
    }

    private void OnDisable() {
        DynamicLinksCallBacks.onShareSocialLink -= ShareSocialLink;
        DynamicLinksCallBacks.onShareLink -= ShareLink;
    }

    private void ShareSocialLink(Uri link, SocialMetaTagParameters socialMetaTagParameters) {
        string msg = socialMetaTagParameters.Title + "\n" + socialMetaTagParameters.Description + "\n" + link.AbsoluteUri;
        ShareLink(msg);
    }

    private void ShareLink(Uri link) {
        ShareLink(link.AbsoluteUri);
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
