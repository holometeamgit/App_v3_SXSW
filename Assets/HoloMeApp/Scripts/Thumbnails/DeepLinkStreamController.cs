using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.Firebase.DynamicLink;
using Firebase.DynamicLinks;
using System;
using Zenject;

/// <summary>
/// Deep Link Controller for StreamData
/// </summary>
public class DeepLinkStreamController : MonoBehaviour {
    [SerializeField]
    private ServerURLAPIScriptableObject _serverURLAPIScriptableObject;
    [SerializeField]
    private VideoUploader _videoUploader;

    private WebRequestHandler _webRequestHandler;

    private const string TITLE = "You have been invited to {0}'s Stadium";
    private const string DESCRIPTION = "Click the link below to join {0}'s Stadium";

    private ShareLinkController _shareController = new ShareLinkController();

    private const string STATUS = "live";
    private const string USERNAME_FILTER = "user__username";
    private const string STATUS_FILTER = "status";

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler) {
        _webRequestHandler = webRequestHandler;
    }


    private void GetStreamBySlug(string slug, Action<long, string> onSuccess, Action<WebRequestError> onFailed = null) {
        _webRequestHandler.Get(GetRequestStreamBySlugURL(slug),
            (code, body) => { onSuccess?.Invoke(code, body); },
            (code, body) => { onFailed?.Invoke(new WebRequestError(code, body)); },
        needHeaderAccessToken: false);
    }

    private void GetStreamByUsername(string username, Action<long, string> onSuccess, Action<WebRequestError> onFailed = null) {
        _webRequestHandler.Get(GetRequestStreamByUsernameURL(username, STATUS),
            (code, body) => { onSuccess?.Invoke(code, body); },
            (code, body) => { onFailed?.Invoke(new WebRequestError(code, body)); },
        needHeaderAccessToken: false);
    }

    private void StreamReceived(string body, Action<StreamJsonData.Data> onSuccess, Action<WebRequestError> onFailed = null) {
        try {
            StreamJsonData.Data data = JsonUtility.FromJson<StreamJsonData.Data>(body);

            HelperFunctions.DevLog("Stream Recieved = " + body);

            onSuccess?.Invoke(data);
        } catch (Exception e) {
            HelperFunctions.DevLogError(e.Message);
            onFailed?.Invoke(new WebRequestError());
        }
    }

    private void StreamsReceived(string body, string username, Action<StreamJsonData.Data> onSuccess, Action<WebRequestError> onFailed = null) {
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
                    onSuccess?.Invoke(lastStreamData);
                }
            } else {
                onFailed?.Invoke(new WebRequestError());
            }
        } catch (Exception e) {
            HelperFunctions.DevLogError(e.Message);
            onFailed?.Invoke(new WebRequestError());
        }
    }

    private void OnOpenStadium(string username) {
        GetStreamByUsername(username,
            (code, body) => {
                OpenStadium(body, username); ;
            }, DeepLinkStadiumConstructor.OnShowError);
    }

    private void OpenStadium(string body, string username) {
        StreamsReceived(body, username,
            (data) => {
                if ((data.GetStage() == StreamJsonData.Data.Stage.Prerecorded && data.HasStreamUrl) || data.GetStage() == StreamJsonData.Data.Stage.Live) {
                    DeepLinkStadiumConstructor.OnShow?.Invoke(data);
                } else {
                    DeepLinkStadiumConstructor.OnShowError?.Invoke(new WebRequestError());
                }
            }, DeepLinkStadiumConstructor.OnShowError);
    }

    private void OnOpenPrerecorded(string slug) {
        GetStreamBySlug(slug,
            (code, body) => {
                OpenPrerecorded(body);
            }, DeepLinkPrerecordedConstructor.OnShowError);
    }

    private void OpenPrerecorded(string body) {
        StreamReceived(body,
            (data) => {
                if ((data.GetStage() == StreamJsonData.Data.Stage.Prerecorded && data.HasStreamUrl) || data.GetStage() == StreamJsonData.Data.Stage.Live) {
                    DeepLinkPrerecordedConstructor.OnShow?.Invoke(data);
                } else {
                    DeepLinkPrerecordedConstructor.OnShowError?.Invoke(new WebRequestError());
                }
            }, DeepLinkPrerecordedConstructor.OnShowError);
    }

    private void Awake() {
        StreamCallBacks.onShareStreamLinkByUsername += OnShare;
        StreamCallBacks.onShareStreamLinkByData += OnShare;
        StreamCallBacks.onReceiveStadiumLink += OnOpenStadium;
        StreamCallBacks.onReceivePrerecordedLink += OnOpenPrerecorded;
    }

    private void OnShare(string username) {
        GetStreamByUsername(username,
            (code, body) => Share(body, username));
    }

    private void Share(string body, string username) {
        StreamsReceived(body, username, OnShare);
    }

    private void OnShare(StreamJsonData.Data data) {

        string msg = string.Empty;

        if (data.HasStreamUrl) {
            msg = data.share_link;
        } else if (data.HasAgoraChannel) {
            string title = string.Format(TITLE, data.user);
            string description = string.Format(DESCRIPTION, data.user);
            msg = title + "\n" + description + "\n" + data.share_link;
        }

        _shareController.ShareLink(msg);
    }


    private void OnDestroy() {
        StreamCallBacks.onShareStreamLinkByUsername -= OnShare;
        StreamCallBacks.onShareStreamLinkByData -= OnShare;
        StreamCallBacks.onReceiveStadiumLink -= OnOpenStadium;
        StreamCallBacks.onReceivePrerecordedLink -= OnOpenPrerecorded;
    }

    private string GetRequestStreamBySlugURL(string slug) {
        return _webRequestHandler.ServerURLMediaAPI + _videoUploader.StreamBySlug.Replace("{slug}", slug);
    }

    private string GetRequestStreamByUsernameURL(string username, string status) {
        return _webRequestHandler.ServerURLMediaAPI + _videoUploader.Stream + $"?{STATUS_FILTER}={status}&{USERNAME_FILTER}={username}";
    }
}
