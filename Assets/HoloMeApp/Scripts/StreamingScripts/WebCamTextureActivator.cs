using TensorFlowLite;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class WebCamTextureActivator : MonoBehaviour {

    private WebCamTexture webCamTexture;
    public WebCamInput.TextureUpdateEvent OnTextureUpdate = new WebCamInput.TextureUpdateEvent();
    ///private Quaternion _baseRotation;
    private Material rawImageMat;

    private RawImage rawImage;
    private AspectRatioFitter imageFitter;

    private void Awake() {
        rawImageMat = GetComponent<RawImage>().materialForRendering;
        rawImage = GetComponent<RawImage>();
        imageFitter = GetComponent<AspectRatioFitter>();
    }

    public void ActivateCamera() {
        if (rawImageMat != null) {
            if (WebCamTexture.devices.Length > 1 && !Application.isEditor) {
                webCamTexture = new WebCamTexture(WebCamTexture.devices[1].name, Screen.width, Screen.height, AgoraSharedVideoConfig.FrameRate); //Set to front camera
            } else {
                webCamTexture = new WebCamTexture();
            }

            rawImageMat.mainTexture = webCamTexture;
            //rawImageMat.SetFloat("_Rotation", webCamTexture.videoRotationAngle * Mathf.PI / 180f);
            //rawImageMat.SetFloat("_Scale", (webCamTexture.videoVerticallyMirrored) ? -1 : 1);

            //transform.localScale = _currectDeviceID == 1 ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
            webCamTexture.Play();
        }
    }

    public void DisableCamera() {
        webCamTexture?.Stop();
    }

    private void Update() {
        if (!webCamTexture.didUpdateThisFrame) {
            return;
        }

        //transform.rotation = _baseRotation * Quaternion.AngleAxis(webCamTexture.videoRotationAngle, Vector3.up);
        OnTextureUpdate.Invoke(webCamTexture);

        if (webCamTexture.width < 100) {
            Debug.Log("Still waiting another frame for correct info...");
            return;
        }

        // change as user rotates iPhone or Android:

        int cwNeeded = webCamTexture.videoRotationAngle;
        // Unity helpfully returns the _clockwise_ twist needed
        // guess nobody at Unity noticed their product works in counterclockwise:
        int ccwNeeded = -cwNeeded;

        // IF the image needs to be mirrored, it seems that it
        // ALSO needs to be spun. Strange: but true.
        if (webCamTexture.videoVerticallyMirrored) ccwNeeded += 180;

        // you'll be using a UI RawImage, so simply spin the RectTransform
        rawImage.rectTransform.localEulerAngles = new Vector3(0f, 0f, ccwNeeded);

        float videoRatio = (float)webCamTexture.width / (float)webCamTexture.height;

        // you'll be using an AspectRatioFitter on the Image, so simply set it
        imageFitter.aspectRatio = videoRatio;

        // alert, the ONLY way to mirror a RAW image, is, the uvRect.
        // changing the scale is completely broken.
        if (webCamTexture.videoVerticallyMirrored)
            rawImage.uvRect = new Rect(1, 0, -1, 1);  // means flip on vertical axis
        else
            rawImage.uvRect = new Rect(0, 0, 1, 1);  // means no flip

    }
}
