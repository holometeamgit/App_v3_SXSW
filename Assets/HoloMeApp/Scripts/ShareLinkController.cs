using UnityEngine;
using NatShare;
using System;
using Firebase.DynamicLinks;

/// <summary>
/// ShareController for links
/// </summary>
public class ShareLinkController {

    /// <summary>
    /// Share Link
    /// </summary>
    /// <param name="link"></param>
    public void ShareLink(string msg) {
#if !UNITY_EDITOR
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyShareEventPressed);
        new NativeShare().SetText(msg).Share();
#else
        HelperFunctions.DevLog(msg);
#endif
    }

    /// <summary>
    /// Share Texture
    /// </summary>
    /// <param name="link"></param>
    public void ShareTexture(Texture2D texture) {
#if !UNITY_EDITOR
        new NativeShare().AddFile(texture).Share();
#else
        HelperFunctions.DevLog(texture.ToString());
#endif
    }

    /// <summary>
    /// Share file
    /// </summary>
    /// <param name="link"></param>
    public void ShareFile(string path) {
#if !UNITY_EDITOR
        new NativeShare().AddFile(path).Share();
#else
        HelperFunctions.DevLog(path);
#endif
    }
}
