using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// ARMsgCameraPreview. Show WebCamTexture on the screen
/// </summary>
[RequireComponent(typeof(RawImage), typeof(AspectRatioFitter))]
public class ARMsgCameraPreview : MonoBehaviour {

    public WebCamTexture cameraTexture { get; private set; }
    private RawImage rawImage;
    private AspectRatioFitter aspectFitter;
    private const int CHECKING_NUMBERS_FOR_MACOS_BUG = 16;
    private Coroutine _coroutine;


    private void OnEnable() {
        _coroutine = StartCoroutine(StartCamera());
    }

    private IEnumerator StartCamera() {
        rawImage = GetComponent<RawImage>();
        aspectFitter = GetComponent<AspectRatioFitter>();

        int width;
        int heigh;
        AgoraSharedVideoConfig.GetResolution(screenWidth: Screen.width, screenHeigh: Screen.height, out width, out heigh);
        // Start the WebCamTexture
        WebCamDevice[] devices = WebCamTexture.devices;
        string devicesName = devices.Length > 1 ? devices[1].name : devices[0].name;
        cameraTexture = new WebCamTexture(devicesName, width, heigh, AgoraSharedVideoConfig.FrameRate);

        cameraTexture.Play();
        yield return new WaitUntil(() => cameraTexture.width != CHECKING_NUMBERS_FOR_MACOS_BUG && cameraTexture.height != CHECKING_NUMBERS_FOR_MACOS_BUG); // Workaround for weird bug on macOS
                                                                                                                                                           // Setup preview shader with correct orientation
        rawImage.texture = cameraTexture;
        rawImage.material.SetFloat("_Rotation", cameraTexture.videoRotationAngle * Mathf.PI / 180f);
        rawImage.material.SetFloat("_Scale", cameraTexture.videoVerticallyMirrored ? -1 : 1);
        // Scale the preview panel
        if (cameraTexture.videoRotationAngle == 90 || cameraTexture.videoRotationAngle == 270) {
            aspectFitter.aspectRatio = (float)cameraTexture.height / cameraTexture.width;
        } else {
            aspectFitter.aspectRatio = (float)cameraTexture.width / cameraTexture.height;
        }
    }

    private void OnDisable() {
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        cameraTexture.Stop();
    }

}