using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.Firebase.DynamicLink;
using Firebase.DynamicLinks;
using System;

public class DeepLinkStreamController : MonoBehaviour {
    [SerializeField] ServerURLAPIScriptableObject serverURLAPIScriptableObject;

    private const string STREAM_TITLE = "Join {0}'s Live Stream";
    private const string STREAM_DESCRIPTION = "Click the link to watch {0} in Augmented Reality.";

    private void Awake() {
        StreamCallBacks.onGetStreamLink += GetStreamLink;
        StreamCallBacks.onGetPrerecordedLink += GetPrerecordedLink;
    }

    private void GetStreamLink(string id, string source) {
        DynamicLinkParameters dynamicLinkParameters = new DynamicLinkParameters(serverURLAPIScriptableObject.FirebaseDynamicLink, serverURLAPIScriptableObject.DesktopUrl, DynamicLinkParameters.Folder.stream, source, id, SocialParameters(source));
        DynamicLinksCallBacks.onCreateShortLink?.Invoke(dynamicLinkParameters);
    }

    private void GetPrerecordedLink(StreamJsonData.Data data) {
        DynamicLinkParameters dynamicLinkParameters = new DynamicLinkParameters(serverURLAPIScriptableObject.FirebaseDynamicLink, serverURLAPIScriptableObject.DesktopUrl, DynamicLinkParameters.Folder.stream, data.user, data.id.ToString(), SocialParameters(data));
        DynamicLinksCallBacks.onCreateShortLink?.Invoke(dynamicLinkParameters);
    }


    private void OnDestroy() {
        StreamCallBacks.onGetStreamLink -= GetStreamLink;
        StreamCallBacks.onGetPrerecordedLink -= GetPrerecordedLink;
    }

    /// <summary>
    /// Social Media for Live Stream
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public SocialMetaTagParameters SocialParameters(string source) {
        SocialMetaTagParameters socialMetaTagParameters = new SocialMetaTagParameters() {
            Title = string.Format(STREAM_TITLE, source),
            Description = string.Format(STREAM_DESCRIPTION, source),
            ImageUrl = new Uri(serverURLAPIScriptableObject.LogoLink)
        };
        return socialMetaTagParameters;
    }

    /// <summary>
    /// Social Media for Prerecorded video
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public SocialMetaTagParameters SocialParameters(StreamJsonData.Data data) {
        SocialMetaTagParameters socialMetaTagParameters = new SocialMetaTagParameters() {
            Title = data.title,
            Description = data.description,
            ImageUrl = new Uri(serverURLAPIScriptableObject.LogoLink)
        };
        return socialMetaTagParameters;
    }
}
