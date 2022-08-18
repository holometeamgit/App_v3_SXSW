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
    private AspectRatioFitter aspectRatioFitter;
    private bool textureSetup;

    private void Awake() {
        rawImageMat = GetComponent<RawImage>().materialForRendering;
        rawImage = GetComponent<RawImage>();
        aspectRatioFitter = GetComponent<AspectRatioFitter>();
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

            //1
            //aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            //if (webCamTexture.videoRotationAngle == 90 || webCamTexture.videoRotationAngle == 270) {//If quad is rotated divide height by width instead
            //    aspectRatioFitter.aspectRatio = (float)webCamTexture.height / webCamTexture.width;
            //} else {
            //    aspectRatioFitter.aspectRatio = (float)webCamTexture.width / webCamTexture.height;
            //}

            //2
            aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.None;
            rawImage.rectTransform.eulerAngles = new Vector3(0, 0, -webCamTexture.videoRotationAngle); //Correct image rotation
            rawImage.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rawImage.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rawImage.SetNativeSize();
            //rawImage.rectTransform.sizeDelta = new Vector2(Screen.height, Screen.width);

            //3
            //aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            //aspectRatioFitter.aspectRatio = (float)webCamTexture.width / webCamTexture.height;
            //rawImage.rectTransform.parent.eulerAngles = new Vector3(0, 0, -webCamTexture.videoRotationAngle); //Correct image rotation on canvas transform
            //if (webCamTexture.videoRotationAngle == 90) {
            //    rawImage.rectTransform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(-Screen.height / 2, Screen.width / 2); //Centre image if rotated
            //}

            //4 Rotate cameras with offset
            //aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            //aspectRatioFitter.aspectRatio = (float)webCamTexture.width / webCamTexture.height;
            //rawImage.rectTransform.parent.parent.Find("StreamRenderTextureCamera").transform.rotation = Quaternion.Euler(0,0,-webCamTexture.videoRotationAngle);
            //rawImage.rectTransform.parent.parent.Find("StreamPreviewCamera").transform.rotation = Quaternion.Euler(0,0,-webCamTexture.videoRotationAngle);

            switch (webCamTexture.videoRotationAngle) {
                case (270):
                    rawImage.uvRect = new Rect(0, 1, 1, -1); //Correct mirroring android
                    break;
                case (90):
                    rawImage.uvRect = new Rect(0, 0, 1, 1);  //Correct mirroring iOS
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
