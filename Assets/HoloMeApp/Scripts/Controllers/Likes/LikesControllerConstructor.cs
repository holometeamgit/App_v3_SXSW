using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;

/// <summary>
/// Constructor for initialization LikesController and adding all subscriptions
/// </summary>
public class LikesControllerConstructor : MonoBehaviour {
    [SerializeField] WebRequestHandler _webRequestHandler;
    [SerializeField] VideoUploader _videoUploader;

    private LikesController _likesController;

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
