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
    private VideoUploader _videoUploader;

    private GetStadiumController _getStadiumController;
    private GetPrerecordedController _getPrerecordedController;

    private const string TITLE = "You have been invited to {0}'s Stadium";
    private const string DESCRIPTION = "Click the link below to join {0}'s Stadium";

    private ShareLinkController _shareController = new ShareLinkController();

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler) {
        _getStadiumController = new GetStadiumController(_videoUploader, webRequestHandler);
        _getPrerecordedController = new GetPrerecordedController(_videoUploader, webRequestHandler);
    }

    private void OnOpenStadium(string username) {
        _getStadiumController.GetStadiumByUsername(username, (data) => {
            if ((data.GetStage() == StreamJsonData.Data.Stage.Prerecorded && data.HasStreamUrl) || data.GetStage() == StreamJsonData.Data.Stage.Live) {
                DeepLinkStadiumConstructor.OnShow?.Invoke(data);
            } else {
                DeepLinkStadiumConstructor.OnShowError?.Invoke(new WebRequestError());
            }
        }, DeepLinkStadiumConstructor.OnShowError);
    }

    private void OnOpenPrerecorded(string slug) {
        _getPrerecordedController.GetPrerecordedBySlug(slug, (data) => {
            if ((data.GetStage() == StreamJsonData.Data.Stage.Prerecorded && data.HasStreamUrl) || data.GetStage() == StreamJsonData.Data.Stage.Live) {
                DeepLinkPrerecordedConstructor.OnShow?.Invoke(data);
            } else {
                DeepLinkPrerecordedConstructor.OnShowError?.Invoke(new WebRequestError());
            }
        }, DeepLinkPrerecordedConstructor.OnShowError);
    }

    private void Awake() {
        StreamCallBacks.onShareStreamLinkByUsername += OnShare;
        StreamCallBacks.onReceiveStadiumLink += OnOpenStadium;
        StreamCallBacks.onReceivePrerecordedLink += OnOpenPrerecorded;
    }

    private void OnShare(string username) {
        _getStadiumController.GetStadiumByUsername(username, OnShare);
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
        StreamCallBacks.onReceiveStadiumLink -= OnOpenStadium;
        StreamCallBacks.onReceivePrerecordedLink -= OnOpenPrerecorded;
    }
}
