using System.Collections;
using TensorFlowLite;
using UnityEngine;
using UnityEngine.UI;

public class PnlStreamMLCameraView : MonoBehaviour {
    [SerializeField]
    private AgoraCustomTextureSender agoraCustomTextureSender;
    [SerializeField]
    private WebCamInput webcamInput;
    [SerializeField]
    private BeemMLHandler beemMLHandler;
    [SerializeField]
    private RawImage imgCameraPreview;
    [SerializeField]
    private Canvas[] canvases;
    [SerializeField]
    private Camera[] renderCameras;

    private bool hasBeenActivated;

    private void Awake() {
        foreach (Camera camera in renderCameras) { //Set the height of the cameras to match the screen size
            camera.orthographicSize = Screen.height / 2;
        }

        foreach (Canvas canvas in canvases) {
            RectTransform canvasRect = canvas.transform.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(Screen.width, Screen.height);//Set the world canvas to correct res in order to see picture
        }
    }

    public void ActivateCameraView() {
        if (hasBeenActivated) {
            return;
        }

        HelperFunctions.DevLog("Activate ML Camera View");
        gameObject.SetActive(true);
        webcamInput.ActivateCamera();
        beemMLHandler.EnableML();
        hasBeenActivated = true;
    }

    public void DisableCameraView() {
        HelperFunctions.DevLog("Disable ML Camera View");
        webcamInput.DisableCamera();
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
