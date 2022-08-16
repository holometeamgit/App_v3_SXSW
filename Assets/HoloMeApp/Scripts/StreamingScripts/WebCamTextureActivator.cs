using System.Collections;
using TensorFlowLite;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class WebCamTextureActivator : MonoBehaviour {

    public WebCamInput.TextureUpdateEvent OnTextureUpdate = new WebCamInput.TextureUpdateEvent();
    private WebCamTexture webCamTexture;
    private Material rawImageMat;
    private RawImage rawImage;
    private AspectRatioFitter imageFitter;
    private bool textureSetup;

    private void Awake() {
        rawImageMat = GetComponent<RawImage>().materialForRendering;
        rawImage = GetComponent<RawImage>();
        imageFitter = GetComponent<AspectRatioFitter>();
    }

    private void OnEnable() {
        StartCoroutine(UpdateTexture());
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    public void ActivateCamera() {
        if (rawImageMat != null) {
            if (WebCamTexture.devices.Length > 1 && !Application.isEditor) {
                webCamTexture = new WebCamTexture(WebCamTexture.devices[1].name, Screen.width, Screen.height, AgoraSharedVideoConfig.FrameRate); //Set to front camera
            } else {
                webCamTexture = new WebCamTexture();
            }

            rawImageMat.mainTexture = webCamTexture;
            webCamTexture.Play();
        }
    }

    public void DisableCamera() {
        webCamTexture?.Stop();
    }

    private void Update() {

        if (!textureSetup) {
            if (webCamTexture == null) {
                return;
            }
            if (!webCamTexture.didUpdateThisFrame) {
                return;
            }

            if (webCamTexture.width < 100) {
                Debug.Log("Still waiting another frame for correct info...");
                return;
            }


            rawImage.rectTransform.eulerAngles = new Vector3(0, 0, -webCamTexture.videoRotationAngle); //Correct image rotation
            imageFitter.aspectMode = AspectRatioFitter.AspectMode.None;
            rawImage.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rawImage.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rawImage.rectTransform.sizeDelta = new Vector2(Screen.height, Screen.width);

            switch (webCamTexture.videoRotationAngle) {
                case (270):
                    rawImage.uvRect = new Rect(0, 1, 1, -1);  // Correct mirroring android
                    break;
                case (90):
                    rawImage.uvRect = new Rect(1, 0, -1, 1);  // Correct mirroring iOS
                    break;
                default:
                    rawImage.uvRect = new Rect(1, 0, -1, 1);
                    break;
            }

            textureSetup = true;
        }

    }

    private IEnumerator UpdateTexture() { //Update at the end of frame to process rotated camera render result
        while (gameObject.activeInHierarchy) {
            yield return new WaitForEndOfFrame();
            OnTextureUpdate.Invoke(rawImageMat.mainTexture); //Use rawImage mat pointing to camera feed result not webcamtexture which may have incorrect orientation
        }
    }
}
