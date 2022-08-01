using TensorFlowLite;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class WebCamTextureActivator : MonoBehaviour {

    private WebCamTexture webCamTexture;
    public WebCamInput.TextureUpdateEvent OnTextureUpdate = new WebCamInput.TextureUpdateEvent();
    private Quaternion _baseRotation;

    void Start() {

        var rawImg = GetComponent<RawImage>().materialForRendering;
        if (rawImg != null) {
            webCamTexture = new WebCamTexture();
            rawImg.mainTexture = webCamTexture;
            webCamTexture.Play();
        }
    }

    private void Update() {
        if (!webCamTexture.didUpdateThisFrame) {
            return;
        }

        transform.rotation = _baseRotation * Quaternion.AngleAxis(webCamTexture.videoRotationAngle, Vector3.up);
        OnTextureUpdate.Invoke(webCamTexture);
    }
}
