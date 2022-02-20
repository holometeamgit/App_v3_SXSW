﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using Zenject;

/// <summary>
/// Manager for initialization ViewsCounterController and adding all subscriptions
/// </summary>
public class ViewsCounterManager : MonoBehaviour {
    [SerializeField]
    private VideoUploader _videoUploader;

    private ViewsCounterController _viewsCounterController;
    private WebRequestHandler _webRequestHandler;

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler) {
        _webRequestHandler = webRequestHandler;
    }

    private void Awake() {
        _viewsCounterController = new ViewsCounterController(_webRequestHandler, _videoUploader);

        CallBacks.onViewed += _viewsCounterController.SendViewed;
    }

    private void OnDestroy() {
        CallBacks.onViewed -= _viewsCounterController.SendViewed;
    }
}
