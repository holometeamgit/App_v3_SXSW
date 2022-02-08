using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.Firebase.DynamicLink;
using Firebase.DynamicLinks;
using System;

/// <summary>
/// Deep Link Controller for StreamData
/// </summary>
public class DeepLinkStreamController : MonoBehaviour {
    [SerializeField]
    private WebRequestHandler _webRequestHandler;
    [SerializeField]
    private ServerURLAPIScriptableObject _serverURLAPIScriptableObject;
    [SerializeField]
    private VideoUploader _videoUploader;

    private ShareLinkController _shareController = new ShareLinkController();

    private const string STREAM_TITLE = "Join {0}'s Live Stream";
    private const string STREAM_DESCRIPTION = "Click the link to watch {0} in Augmented Reality.";
    private const string STATUS = "live";
    private const string USERNAME_FILTER = "user__username";
    private const string STATUS_FILTER = "status";

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
                ImageUrl = new Uri(_serverURLAPIScriptableObject.LogoLink)
            };
        } else {
            socialMetaTagParameters = new SocialMetaTagParameters() {
                Title = data.title,
                Description = data.description,
                ImageUrl = new Uri(_serverURLAPIScriptableObject.LogoLink)
            };
        }
        return socialMetaTagParameters;
    }

    private void GetStreamBySlug(string slug, Action<long, string> onSuccess, Action<long, string> onFailed) {
        _webRequestHandler.Get(GetRequestStreamBySlugURL(slug),
            (code, body) => { onSuccess?.Invoke(code, body); },
            (code, body) => { onFailed?.Invoke(code, body); },
        needHeaderAccessToken: false);
    }

    private void GetStreamByUsername(string username, Action<long, string> onSuccess, Action<long, string> onFailed) {
        _webRequestHandler.Get(GetRequestStreamByUsernameURL(username, STATUS),
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

    private void StreamsReceived(string body, string username, Action<StreamJsonData.Data> onReceived) {
        try {
            HelperFunctions.DevLog("Streams Recieved = " + body);

            StreamJsonData data = JsonUtility.FromJson<StreamJsonData>(body);
            if (data.results.Count > 0) {

                StreamJsonData.Data lastStreamData = null;

                foreach (StreamJsonData.Data item in data.results) {
                    if (lastStreamData != null) {
                        if (item.StartDate.CompareTo(lastStreamData.StartDate) < 0 && item.user == username) {
                            lastStreamData = item;
                        }
                    } else {
                        if (item.user == username) {
                            lastStreamData = item;
                        }
                    }
                }

                if (lastStreamData != null) {
                    onReceived?.Invoke(lastStreamData);
                }
            }
        } catch (Exception e) {
            HelperFunctions.DevLogError(e.Message);
        }
    }

    private void OnOpenStream(string username) {
        GetStreamByUsername(username,
            (code, body) => {
                OpenStream(body, username); ;
            },
            (code, body) => {
                HelperFunctions.DevLogError(code + " " + body);
            });
    }

    private void OpenStream(string body, string username) {
        StreamsReceived(body,
            username,
            (data) => {
                StreamCallBacks.onStreamDataReceived?.Invoke(data);
            });
    }

    private void OnOpenPrerecorded(string slug) {
        GetStreamBySlug(slug,
            (code, body) => {
                OpenPrerecorded(body); ;
            },
            (code, body) => {
                HelperFunctions.DevLogError(code + " " + body);
            });
    }

    private void OpenPrerecorded(string body) {
        StreamReceived(body,
            (data) => {
                StreamCallBacks.onStreamDataReceived?.Invoke(data);
            });
    }

    private void Awake() {
        StreamCallBacks.onShareStreamLinkByUsername += OnShare;
        StreamCallBacks.onShareStreamLinkByData += OnShare;
        StreamCallBacks.onReceiveStreamLink += OnOpenStream;
        StreamCallBacks.onReceivePrerecordedLink += OnOpenPrerecorded;
    }

    private void OnShare(string username) {
        GetStreamByUsername(username,
            (code, body) => Share(body, username),
            (code, body) => {
                HelperFunctions.DevLogError(code + " " + body);
            });
    }

    private void Share(string body, string username) {
        StreamsReceived(body,
            username,
            (data) => {
                OnShare(data);
            });
    }

    private void OnShare(StreamJsonData.Data data) {
        _shareController.ShareSocialLink(new Uri(data.share_link), SocialParameters(data));
    }


    private void OnDestroy() {
        StreamCallBacks.onShareStreamLinkByUsername -= OnShare;
        StreamCallBacks.onShareStreamLinkByData -= OnShare;
        StreamCallBacks.onReceiveStreamLink -= OnOpenStream;
        StreamCallBacks.onReceivePrerecordedLink -= OnOpenPrerecorded;
    }

    private string GetRequestStreamBySlugURL(string slug) {
        return _webRequestHandler.ServerURLMediaAPI + _videoUploader.StreamBySlug.Replace("{slug}", slug);
    }

    private string GetRequestStreamByUsernameURL(string username, string status) {
        return _webRequestHandler.ServerURLMediaAPI + _videoUploader.Stream + $"?{STATUS_FILTER}={status}&{USERNAME_FILTER}={username}";
    }
}
