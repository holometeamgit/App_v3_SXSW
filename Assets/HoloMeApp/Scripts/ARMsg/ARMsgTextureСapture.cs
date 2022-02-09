using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARMsgTextureСapture : MonoBehaviour {

    [SerializeField]
    ARMsgCameraPreview _arMsgCameraPreview;
    [SerializeField]
    RawImage _rawImage;
    [SerializeField]
    AspectRatioFitter _aspectRatioFitter;

    private void Awake() {
        _arMsgCameraPreview.onTextureUpdated += UpdateDataRendering;
    }

    private void UpdateDataRendering() {
        _rawImage.texture = _arMsgCameraPreview.GetTexture();
        _aspectRatioFitter.aspectRatio = _arMsgCameraPreview.GetAspectRatio();

        HelperFunctions.DevLog("_rawImage.texture = " + _rawImage.texture);
    }

    private void OnDestroy() {
        _arMsgCameraPreview.onTextureUpdated -= UpdateDataRendering;
    }
}
