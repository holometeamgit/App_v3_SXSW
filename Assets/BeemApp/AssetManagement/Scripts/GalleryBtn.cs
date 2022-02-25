using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gallery Btn
/// </summary>
public class GalleryBtn : MonoBehaviour {
    [SerializeField]
    private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;

    [SerializeField]
    private bool open = true;

    private WebRequestHandler _webRequestHandler;

    private WebRequestHandler GetWebRequestHandler {
        get {

            if (_webRequestHandler == null) {
                _webRequestHandler = FindObjectOfType<WebRequestHandler>();
            }

            return _webRequestHandler;
        }
    }

    private GalleryController _galleryController;

    private void Start() {
        _galleryController = new GalleryController(_arMsgAPIScriptableObject, GetWebRequestHandler);
    }

    /// <summary>
    /// On Click
    /// </summary>
    public void OnClick() {
        if (open) {
            _galleryController.GetAllArMessages(onSuccess: Show);
        } else {
            Hide();
        }
    }

    private void Show(ARMsgJSON data) {
        StreamOverlayConstructor.onDeactivatedAsBroadcaster?.Invoke();
        ARMsgRecordConstructor.OnActivated?.Invoke(false);
        GalleryNotificationConstructor.OnHide?.Invoke();
        MenuConstructor.OnActivated?.Invoke(false);
        GalleryConstructor.OnShow?.Invoke(data);
    }

    private void Hide() {
        GalleryConstructor.OnHide?.Invoke();
        MenuConstructor.OnActivated?.Invoke(true);
        ARMsgRecordConstructor.OnActivated?.Invoke(true);
    }
}
