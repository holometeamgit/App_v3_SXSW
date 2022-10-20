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
    private Canvas senderGreenBGCanvas;
    [SerializeField]
    private Canvas noGreenBGCanvas;
    [SerializeField]
    private Camera renderTextureCamera;
    [SerializeField]
    private Camera previewCamera;

    private const string GREEN_BG_LAYER = "MlResult";
    private const string NO_GREEN_BG_LAYER = "CamPreviewNoML";

    private bool hasBeenActivated;

    private void Awake() {
        SetCameraSize(renderTextureCamera);
        SetCameraSize(previewCamera);
        SetCanvasSize(senderGreenBGCanvas);
        SetCanvasSize(noGreenBGCanvas);
        EnableGreenBG();
    }

    /// <summary>
    /// Set the height of the cameras to match the screen size
    /// </summary>
    private void SetCameraSize(Camera camera) {
        camera.orthographicSize = Screen.height / 2;
    }

    /// <summary>
    /// Corrects the canvases resolution
    /// </summary>
    private void SetCanvasSize(Canvas canvas) {
        RectTransform canvasRect = canvas.transform.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(Screen.width, Screen.height);//Set the world canvas to correct res in order to see picture
    }

    /// <summary>
    /// Call to enable green background effect
    /// </summary>
    public void EnableGreenBG() {
        previewCamera.cullingMask = 1 << LayerMask.NameToLayer(GREEN_BG_LAYER);
        noGreenBGCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// Disable green background effect and display raw camera background
    /// </summary>
    public void DisableGreenBG() {
        previewCamera.cullingMask = 1 << LayerMask.NameToLayer(NO_GREEN_BG_LAYER);
        noGreenBGCanvas.gameObject.SetActive(true);
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
