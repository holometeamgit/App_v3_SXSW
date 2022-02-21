using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Beem.ARMsg;

/// <summary>
/// The class is responsible for changing the text depending on the selected phone camera
/// </summary>
public class ARMsgTextHelp : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _text;

    [SerializeField]
    [TextArea]
    private string _stringForBackCamera =
        "The recording will start on the next\n" +
        "screen.Make sure the person you are\n" +
        "recording is in full frame throughout";

    [SerializeField]
    [TextArea]
    private string _stringForFrontCamera = "";


    private void OnEnable() {
        CallBacks.onCameraSwitched += UpdateText;
        if(CallBacks.onGetCurrentCameraID != null)
            UpdateText(CallBacks.onGetCurrentCameraID.Invoke());
    }

    private void UpdateText(int cameraID) {
        if(cameraID == ARMsgCameraPreview.BACK_CAMERA) {
            _text.text = _stringForBackCamera;
        } else {
            _text.text = _stringForFrontCamera;
        }
    }

    private void OnDisable() {
        CallBacks.onCameraSwitched -= UpdateText;
    }
}
