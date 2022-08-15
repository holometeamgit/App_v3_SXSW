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

    [SerializeField]
    private Canvas textureStreamCanvas;

    [SerializeField]
    private Camera[] renderCameras;

    private bool hasBeenActivated;

    private void Awake() {
        textureStreamCanvas.renderMode = RenderMode.WorldSpace; //By starting canvas in screenspace Unity adjust viewport to fit perfectly with screen size, this allows accurate canvas size in world space
        RectTransform rTrans = textureStreamCanvas.GetComponent<RectTransform>();
        rTrans.anchoredPosition = Vector2.zero;

        foreach(Camera camera in renderCameras) { //Set the height of the cameras to match the screen size
            camera.orthographicSize = Screen.height / 2;
        }
    }

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
        agoraCustomTextureSender.StartSendingTextureUpdates();
    }

    public void StopSendingCustomTexture() {
        agoraCustomTextureSender.StopSendingTextureRoutine();
    }
}
