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

    private const string TITLE = "You have been invited to {0}'s Stadium";
    private const string DESCRIPTION = "Click the link below to join {0}'s Stadium";

    private ShareLinkController _shareController = new ShareLinkController();

    private const string STATUS = "live";
    private const string USERNAME_FILTER = "user__username";
    private const string STATUS_FILTER = "status";


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
                if ((data.GetStage() == StreamJsonData.Data.Stage.Prerecorded && data.HasStreamUrl) || data.GetStage() == StreamJsonData.Data.Stage.Live) {
                    DeepLinkStreamConstructor.OnShow?.Invoke(data);
                }
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
                if ((data.GetStage() == StreamJsonData.Data.Stage.Prerecorded && data.HasStreamUrl) || data.GetStage() == StreamJsonData.Data.Stage.Live) {
                    DeepLinkStreamConstructor.OnShow?.Invoke(data);
                }
            });
    }

    private void Awake() {
        StreamCallBacks.onShareStreamLinkByUsername += OnShare;
        StreamCallBacks.onShareStreamLinkByData += OnShare;
        StreamCallBacks.onReceiveStadiumLink += OnOpenStream;
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
        StreamCallBacks.onReceiveStadiumLink -= OnOpenStream;
        StreamCallBacks.onReceivePrerecordedLink -= OnOpenPrerecorded;
    }

    private string GetRequestStreamBySlugURL(string slug) {
        return _webRequestHandler.ServerURLMediaAPI + _videoUploader.StreamBySlug.Replace("{slug}", slug);
    }

    private string GetRequestStreamByUsernameURL(string username, string status) {
        return _webRequestHandler.ServerURLMediaAPI + _videoUploader.Stream + $"?{STATUS_FILTER}={status}&{USERNAME_FILTER}={username}";
    }
}
