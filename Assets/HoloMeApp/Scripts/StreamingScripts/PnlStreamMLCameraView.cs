using UnityEngine;
using UnityEngine.UI;

public class PnlStreamMLCameraView : MonoBehaviour {
    [SerializeField]
    private AgoraCustomTextureSender agoraCustomTextureSender;

    [SerializeField]
    private WebCamTextureActivator webCamTextureActivator;

    [SerializeField]
    private BeemMLHandler beemMLHandler;

    [SerializeField]
    private RawImage imgCameraPreview;

    private bool hasBeenActivated;

    public void ActivateCameraView() {
        if (hasBeenActivated) {
            return;
        }

        hasBeenActivated = true;
        gameObject.SetActive(true);
        HelperFunctions.DevLog("Activate ML Camera View");
        webCamTextureActivator.ActivateCamera();
        beemMLHandler.EnableML();
    }

    public void DisableCameraView() {
        HelperFunctions.DevLog("Disable ML Camera View");
        webCamTextureActivator.DisableCamera();
        gameObject.SetActive(false);
        hasBeenActivated = false;
    }

    public void StartSendingCustomTexture() {
        agoraCustomTextureSender.StartSendingTexture = true;
    }

    public void StopSendingCustomTexture() {
        agoraCustomTextureSender.StartSendingTexture = false;
    }
}
