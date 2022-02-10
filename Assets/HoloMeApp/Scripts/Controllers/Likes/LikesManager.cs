using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using Zenject;

/// <summary>
/// Manager for initialization LikesController and adding all subscriptions
/// </summary>
public class LikesManager : MonoBehaviour {
    [SerializeField] VideoUploader _videoUploader;

    private LikesController _likesController;
    private WebRequestHandler _webRequestHandler;

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler) {
        _webRequestHandler = webRequestHandler;
    }

    private void Awake() {
        _likesController = new LikesController(_webRequestHandler, _videoUploader);

        CallBacks.onClickLike += _likesController.SendRequestLike;
        CallBacks.onClickUnlike += _likesController.SendRequestUnlike;
    }

    private void OnDestroy() {
        CallBacks.onClickLike -= _likesController.SendRequestLike;
        CallBacks.onClickUnlike -= _likesController.SendRequestUnlike;
    }
}
