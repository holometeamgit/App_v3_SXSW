using TensorFlowLite;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class WebCamTextureActivator : MonoBehaviour {

    private WebCamTexture webCamTexture;
    public WebCamInput.TextureUpdateEvent OnTextureUpdate = new WebCamInput.TextureUpdateEvent();
    private Quaternion _baseRotation;
    private Material rawImageMat;

    private void Awake() {
        rawImageMat = GetComponent<RawImage>().materialForRendering;
    }

    public void ActivateCamera() {
        if (rawImageMat != null) {
            if (WebCamTexture.devices.Length > 1) {
                webCamTexture = new WebCamTexture(WebCamTexture.devices[1].name); //Set to front camera
            } else {
                webCamTexture = new WebCamTexture();
            }

            rawImageMat.mainTexture = webCamTexture;
            rawImageMat.SetFloat("_Rotation", webCamTexture.videoRotationAngle * Mathf.PI / 180f);
            rawImageMat.SetFloat("_Scale", (webCamTexture.videoVerticallyMirrored) ? -1 : 1);

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

        transform.rotation = _baseRotation * Quaternion.AngleAxis(webCamTexture.videoRotationAngle, Vector3.up);
        OnTextureUpdate.Invoke(webCamTexture);
    }
}
