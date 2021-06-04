using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;

public class ViewsCounterConstructor : MonoBehaviour
{
    [SerializeField] WebRequestHandler _webRequestHandler;
    [SerializeField] VideoUploader _videoUploader;

    private ViewsCounterController _viewsCounterController;

    private void Awake() {
        _viewsCounterController = new ViewsCounterController(_webRequestHandler, _videoUploader);

        CallBacks.onViewed += _viewsCounterController.SendViewed;
    }

    private void OnDestroy() {
        CallBacks.onViewed -= _viewsCounterController.SendViewed;
    }
}
