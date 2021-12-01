using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.Firebase.DynamicLink;
using Firebase.DynamicLinks;
using System;

public class DeepLinkStreamController : MonoBehaviour {
    [SerializeField] WebRequestHandler webRequestHandler;
    [SerializeField] ServerURLAPIScriptableObject serverURLAPIScriptableObject;
    [SerializeField] VideoUploader videoUploader;

    private const string STREAM_TITLE = "Join {0}'s Live Stream";
    private const string STREAM_DESCRIPTION = "Click the link to watch {0} in Augmented Reality.";
    private const string STATUS = "live";

    private void GetStreamById(string id, Action<long, string> onSuccess, Action<long, string> onFailed) {
        webRequestHandler.Get(GetRequestStreamByIdURL(id),
            (code, body) => { onSuccess?.Invoke(code, body); },
            (code, body) => { onFailed?.Invoke(code, body); },
        needHeaderAccessToken: true);
    }

    private void GetStreamBySlug(string slug, Action<long, string> onSuccess, Action<long, string> onFailed) {
        webRequestHandler.Get(GetRequestStreamBySlugURL(slug),
            (code, body) => { onSuccess?.Invoke(code, body); },
            (code, body) => { onFailed?.Invoke(code, body); },
        needHeaderAccessToken: false);
    }

    private void GetStreamByUsername(string username, Action<long, string> onSuccess, Action<long, string> onFailed) {
        webRequestHandler.Get(GetRequestStreamByUsernameURL(username, STATUS),
            (code, body) => { onSuccess?.Invoke(code, body); },
            (code, body) => { onFailed?.Invoke(code, body); },
        needHeaderAccessToken: false);
    }

    private void StreamReceived(string body, Action<StreamJsonData.Data> onReceived) {
        try {
            StreamJsonData.Data data = JsonUtility.FromJson<StreamJsonData.Data>(body);

            HelperFunctions.DevLog("Stream Recieved = " + body);

            onReceived?.Invoke(data);
        } catch (Exception e) {
            HelperFunctions.DevLogError(e.Message);
        }
    }

    private void Awake() {
        StreamCallBacks.onShareStreamLinkById += OnShare;
        StreamCallBacks.onShareStreamLinkByData += OnShare;
    }

    private void OnShare(string id) {
        GetStreamById(id,
            (code, body) => Share(body),
            (code, body) => {
                HelperFunctions.DevLogError(code + " " + body);
            });
    }

    private void Share(string body) {
        StreamReceived(body,
            (data) => {
                OnShare(data);
            });
    }

    private void OnShare(StreamJsonData.Data data) {
        DynamicLinkParameters dynamicLinkParameters = new DynamicLinkParameters(serverURLAPIScriptableObject.FirebaseDynamicLink, serverURLAPIScriptableObject.DesktopUrl, DynamicLinkParameters.Folder.stream, data.user, data.id.ToString(), SocialParameters(data));
        //DynamicLinksCallBacks.onShareSocialLink?.Invoke(new Uri(data.share_link), SocialParameters(data));
        DynamicLinksCallBacks.onCreateShortLink?.Invoke(dynamicLinkParameters);
    }


    private void OnDestroy() {
        StreamCallBacks.onShareStreamLinkById -= OnShare;
        StreamCallBacks.onShareStreamLinkByData -= OnShare;
    }

    private string GetRequestStreamByIdURL(string id) {
        return webRequestHandler.ServerURLMediaAPI + videoUploader.StreamById.Replace("{id}", id);
    }

    private string GetRequestStreamBySlugURL(string slug) {
        return webRequestHandler.ServerURLMediaAPI + videoUploader.StreamBySlug.Replace("{slug}", slug);
    }

    private string GetRequestStreamByUsernameURL(string username, string status) {
        return webRequestHandler.ServerURLMediaAPI + videoUploader.Stream + $"?status={status}&username={username}";
    }

    /// <summary>
    /// Social Media for Streams
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public SocialMetaTagParameters SocialParameters(StreamJsonData.Data data) {
        SocialMetaTagParameters socialMetaTagParameters;
        if (data.GetStage() == StreamJsonData.Data.Stage.Live) {
            socialMetaTagParameters = new SocialMetaTagParameters() {
                Title = string.Format(STREAM_TITLE, data.user),
                Description = string.Format(STREAM_DESCRIPTION, data.user),
                ImageUrl = new Uri(serverURLAPIScriptableObject.LogoLink)
            };
        } else {
            socialMetaTagParameters = new SocialMetaTagParameters() {
                Title = data.title,
                Description = data.description,
                ImageUrl = new Uri(serverURLAPIScriptableObject.LogoLink)
            };
        }
        return socialMetaTagParameters;
    }
}
