using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;

public class LikesControllerConstructor : MonoBehaviour {
    [SerializeField] WebRequestHandler _webRequestHandler;
    [SerializeField] VideoUploader _videoUploader;

    private LikesController likesController;

    private void Awake() {
        likesController = new LikesController(_webRequestHandler, _videoUploader);

        CallBacks.onClickLike += likesController.SendRequestLike;
        CallBacks.onClickUnlike += likesController.SendRequestUnlike;
    }

    private void OnDestroy() {
        CallBacks.onClickLike -= likesController.SendRequestLike;
        CallBacks.onClickUnlike -= likesController.SendRequestUnlike;
    }
}
