using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(RawImage), typeof(AspectRatioFitter))]
public class ARMsgCameraPreview : MonoBehaviour {

    public WebCamTexture cameraTexture { get; private set; }
    private RawImage rawImage;
    private AspectRatioFitter aspectFitter;

    private void OnEnable() {
        StartCoroutine(StartCamera());
    }

    private IEnumerator StartCamera() {
        rawImage = GetComponent<RawImage>();
        aspectFitter = GetComponent<AspectRatioFitter>();
        // Request camera permission
        if (Application.platform == RuntimePlatform.Android) {
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera)) {
                Permission.RequestUserPermission(Permission.Camera);
                yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission(Permission.Camera));
            }
        } else {
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
                yield break;
        }
        int width;
        int heigh;
        AgoraSharedVideoConfig.GetResolution(screenWidth: Screen.width, screenHeigh: Screen.height, out width, out heigh);
        // Start the WebCamTexture
        WebCamDevice[] devices = WebCamTexture.devices;
        string devicesName = devices.Length > 1 ? devices[1].name : devices[0].name;
        cameraTexture = new WebCamTexture(devicesName, width, heigh, AgoraSharedVideoConfig.FrameRate);

        cameraTexture.Play();
        yield return new WaitUntil(() => cameraTexture.width != 16 && cameraTexture.height != 16); // Workaround for weird bug on macOS
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
        cameraTexture.Stop();
    }

}