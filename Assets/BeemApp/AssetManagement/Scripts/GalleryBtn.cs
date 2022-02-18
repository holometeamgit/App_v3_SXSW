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
    private WebRequestHandler _webRequestHandler;

    private GalleryController _galleryController;

    private void Start() {
        _galleryController = new GalleryController(_arMsgAPIScriptableObject, _webRequestHandler);
    }

    /// <summary>
    /// On Click
    /// </summary>
    public void OnClick() {
        _galleryController.GetAllArMessages(onSuccess: GalleryConstructor.OnShow);
    }
}
