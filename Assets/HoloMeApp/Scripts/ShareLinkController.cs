using UnityEngine;
using NatShare;
using System;
using Firebase.DynamicLinks;

/// <summary>
/// ShareController for links
/// </summary>
public class ShareLinkController {

    /// <summary>
    /// Share Social Link
    /// </summary>
    /// <param name="link"></param>
    /// <param name="socialMetaTagParameters"></param>
    public void ShareSocialLink(Uri link, SocialMetaTagParameters socialMetaTagParameters) {
        string msg = socialMetaTagParameters.Title + "\n" + socialMetaTagParameters.Description + "\n" + link.AbsoluteUri;
        ShareLink(msg);
    }

    /// <summary>
    /// Share Link
    /// </summary>
    /// <param name="link"></param>
    public void ShareLink(Uri link) {
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
