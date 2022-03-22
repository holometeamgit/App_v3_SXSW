using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using System.Collections;
using Beem.ARMsg;
using System;
using System.Threading.Tasks;
using System.Threading;

/// <summary>
/// ARMsgCameraPreview. Show WebCamTexture on the screen
/// </summary>
[RequireComponent(typeof(RawImage), typeof(AspectRatioFitter))]
public class ARMsgCameraPreview : MonoBehaviour {

    public static int FRONT_CAMERA = 1;
    public static int BACK_CAMERA = 0;

    public WebCamTexture cameraTexture { get; private set; }
    private RawImage rawImage;
    private AspectRatioFitter aspectFitter;
    private const int CHECKING_NUMBERS_FOR_MACOS_BUG = 16;
    private Coroutine _coroutine;
    private int _currectDeviceID = 0;
    private string _devicesName;

    public Action onTextureUpdated;

    public Texture GetTexture() {
        return rawImage.texture;
    }

    public float GetAspectRatio() {
        return aspectFitter.aspectRatio;
    }

    private void Awake() {
        CallBacks.onCanSwitchCamera += CanSwitchCamera;
        CallBacks.onSwitchCameraClicked += SwitchCamera;
        SwitchDevice();
    }

    private bool CanSwitchCamera() {
        return WebCamTexture.devices.Length > 1;
    }

    private void SwitchCamera() {
        if (cameraTexture != null && cameraTexture.isPlaying)
            cameraTexture.Stop();

        SwitchDevice();

        StopStartCameraCoroutine();
        StartStartCameraCoroutine();
    }

    private void SwitchDevice() {
        WebCamDevice[] devices = WebCamTexture.devices;
        HelperFunctions.DevLogError($"devices = {devices.Length}");
        if (devices.Length > 1)
            _currectDeviceID = (_currectDeviceID + 1) % 2;

        _devicesName = devices[_currectDeviceID].name;
        HelperFunctions.DevLogError($"_devicesName = {_devicesName}");
        CallBacks.onCameraSwitched?.Invoke(_currectDeviceID);
    }

    private void StartStartCameraCoroutine() {
        _coroutine = StartCoroutine(StartCamera());
    }

    private void StopStartCameraCoroutine() {
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private int GetCurrentCameraID() {
        return _currectDeviceID;
    }


    private void OnEnable() {
        if (_currectDeviceID != 1) {
            SwitchDevice();
        }

        StartStartCameraCoroutine();
        CallBacks.onGetCurrentCameraID += GetCurrentCameraID;
    }

    private void OnDisable() {
        StopStartCameraCoroutine();
        if (cameraTexture != null)
            cameraTexture.Stop();
        CallBacks.onGetCurrentCameraID -= GetCurrentCameraID;
    }

    private void OnDestroy() {
        CallBacks.onCanSwitchCamera -= CanSwitchCamera;
        CallBacks.onSwitchCameraClicked -= SwitchCamera;
    }

    private IEnumerator StartCamera() {
        rawImage = GetComponent<RawImage>();
        aspectFitter = GetComponent<AspectRatioFitter>();

        int width;
        int heigh;
        AgoraSharedVideoConfig.GetResolution(screenWidth: Screen.width, screenHeigh: Screen.height, out width, out heigh);
        // Start the WebCamTexture

        cameraTexture = new WebCamTexture(_devicesName, width, heigh, AgoraSharedVideoConfig.FrameRate);

        cameraTexture.Play();
        yield return new WaitUntil(() => cameraTexture.width != CHECKING_NUMBERS_FOR_MACOS_BUG && cameraTexture.height != CHECKING_NUMBERS_FOR_MACOS_BUG); // Workaround for weird bug on macOS
                                                                                                                                                           // Setup preview shader with correct orientation
        rawImage.texture = cameraTexture;
        rawImage.material.SetFloat("_Rotation", cameraTexture.videoRotationAngle * Mathf.PI / 180f);
        rawImage.material.SetFloat("_Scale", (cameraTexture.videoVerticallyMirrored) ? -1 : 1);
        transform.localScale = _currectDeviceID == 1 ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
        // Scale the preview panel
        if (cameraTexture.videoRotationAngle == 90 || cameraTexture.videoRotationAngle == 270) {
            aspectFitter.aspectRatio = (float)cameraTexture.height / cameraTexture.width;
        } else {
            aspectFitter.aspectRatio = (float)cameraTexture.width / cameraTexture.height;
        }

        onTextureUpdated?.Invoke();
    }

}