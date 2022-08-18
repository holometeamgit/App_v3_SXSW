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
    private Canvas canvas;
    [SerializeField]
    private Camera[] renderCameras;

    private bool hasBeenActivated;

    private void Awake() {
        foreach (Camera camera in renderCameras) { //Set the height of the cameras to match the screen size
            camera.orthographicSize = Screen.height / 2;
        }

        RectTransform canvasRect = canvas.transform.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(Screen.width, Screen.height);//Set the world canvas to correct res in order to see picture
        //canvasRect.anchoredPosition = new Vector2(-Screen.width / 2, -Screen.height / 2);//Centre canvas to cameras
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
