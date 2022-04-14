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

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler) {
        _getStadiumController = new GetStadiumController(_videoUploader, webRequestHandler);
        _getPrerecordedController = new GetPrerecordedController(_videoUploader, webRequestHandler);
    }

    private void OnOpenStadium(string username) {
        _getStadiumController.GetStadiumByUsername(username, DeepLinkStadiumConstructor.OnShow, DeepLinkStadiumConstructor.OnShowError);
    }

    private void OnOpenPrerecorded(string slug) {
        _getPrerecordedController.GetPrerecordedBySlug(slug, DeepLinkPrerecordedConstructor.OnShow, DeepLinkPrerecordedConstructor.OnShowError);
    }

    private void Awake() {
        StreamCallBacks.onReceiveStadiumLink += OnOpenStadium;
        StreamCallBacks.onReceivePrerecordedLink += OnOpenPrerecorded;
    }

    private void OnDestroy() {
        StreamCallBacks.onReceiveStadiumLink -= OnOpenStadium;
        StreamCallBacks.onReceivePrerecordedLink -= OnOpenPrerecorded;
    }
}
