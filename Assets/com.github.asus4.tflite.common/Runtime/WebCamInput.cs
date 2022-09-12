using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace TensorFlowLite
{
    /// <summary>
    /// An wrapper for WebCamTexture that corrects texture rotation
    /// </summary>
    public sealed class WebCamInput : MonoBehaviour
    {
        [System.Serializable]
        public class TextureUpdateEvent : UnityEvent<Texture> { }

        [SerializeField, WebCamName] private string editorCameraName;
        [SerializeField] private WebCamKind preferKind = WebCamKind.WideAngle;
        [SerializeField] private bool isFrontFacing = false;
        [SerializeField] private Vector2Int requestSize = new Vector2Int(1280, 720);
        [SerializeField] private int requestFps = 60;

        [SerializeField] private Material cameraViewMat;
        [SerializeField] private RawImage camerViewRenderRawImage;
        [SerializeField] private RawImage outputViewRawImage;
        private RenderTexture renTexCamView;
        
        public TextureUpdateEvent OnTextureUpdate = new TextureUpdateEvent();

        private TextureResizer _resizer;
        private WebCamTexture _webCamTexture;
        private WebCamDevice[] _devices;
        private int _deviceIndex;

        private bool hasActivated;

        private void Activate()
        {
            if (hasActivated)
                return;

            _resizer = new TextureResizer();
            _devices = WebCamTexture.devices;
            var cameraName = Application.isEditor
                ? editorCameraName
                : WebCamUtil.FindName(preferKind, isFrontFacing);

            WebCamDevice device = default;
            for (var i = 0; i < _devices.Length; i++)
            {
                if (_devices[i].name != cameraName)
                {
                    continue;
                }
                
                device = _devices[i];
                _deviceIndex = i;
                
                break;
            }
            hasActivated = true;
        }

        public void ActivateCamera() {
            Activate();
            StartCamera(_devices[_deviceIndex]);
        }

        public void DisableCamera() {
            StopCamera();
        }
          
        private void OnDestroy()
        {
            StopCamera();
            _resizer?.Dispose();
        }

        private void Update()
        {
            if (_webCamTexture == null) {
                return;
            }

            if (!_webCamTexture.didUpdateThisFrame)
            {
                return;
            }

            BlitTex();

            var tex = NormalizeWebcam(_webCamTexture, Screen.width, Screen.height, isFrontFacing);
            OnTextureUpdate?.Invoke(tex);
        }


        private void BlitTex() {

            if(renTexCamView == null) { //Create rendertexture at screen resolution
                renTexCamView = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
                renTexCamView.Create();
                RenderTexture.active = renTexCamView;
            }

            var texture2D = new Texture2D(renTexCamView.width, renTexCamView.height, TextureFormat.RGBA32, false);
            var currentRT = RenderTexture.active;

            RenderTexture.active = renTexCamView;
            texture2D.ReadPixels(new Rect(0, 0, renTexCamView.width, renTexCamView.height), 0, 0);
            Graphics.Blit(camerViewRenderRawImage.texture, renTexCamView, cameraViewMat);
            texture2D.Apply();


            RenderTexture.active = currentRT;
            outputViewRawImage.texture = texture2D;
        }

        // Invoked by Unity Event
        [Preserve]
        public void ToggleCamera()
        {
            _deviceIndex = (_deviceIndex + 1) % _devices.Length;
            StartCamera(_devices[_deviceIndex]);
        }

        private void StartCamera(WebCamDevice device)
        {
            StopCamera();
            isFrontFacing = device.isFrontFacing;
            
            _webCamTexture = new WebCamTexture(device.name, requestSize.x, requestSize.y, requestFps);
            _webCamTexture.Play();
        }

        private void StopCamera()
        {
            if (_webCamTexture == null)
            {
                return;
            }
            
            _webCamTexture.Stop();
            Destroy(_webCamTexture);
        }

        private RenderTexture NormalizeWebcam(WebCamTexture texture, int width, int height, bool isFrontFacing)
        {
            var cameraWidth = texture.width;
            var cameraHeight = texture.height;
            var isPortrait = IsPortrait(texture);
            
            if (isPortrait)
            {
                (cameraWidth, cameraHeight) = (cameraHeight, cameraWidth); // swap
            }

            var cameraAspect = (float)cameraWidth / cameraHeight;
            var targetAspect = (float)width / height;

            int w, h;
            if (cameraAspect > targetAspect)
            {
                w = RoundToEven(cameraHeight * targetAspect);
                h = cameraHeight;
            }
            else
            {
                w = cameraWidth;
                h = RoundToEven(cameraWidth / targetAspect);
            }

            Matrix4x4 mtx;
            Vector4 uvRect;
            var rotation = texture.videoRotationAngle;

            // Seems to be bug in the android. might be fixed in the future.
            if (Application.platform == RuntimePlatform.Android)
            {
                rotation = -rotation;
            }

            if (isPortrait)
            {
                mtx = TextureResizer.GetVertTransform(rotation, texture.videoVerticallyMirrored, isFrontFacing);
                uvRect = TextureResizer.GetTextureST(targetAspect, cameraAspect, AspectMode.Fill);
            }
            else
            {
                mtx = TextureResizer.GetVertTransform(rotation, isFrontFacing, texture.videoVerticallyMirrored);
                uvRect = TextureResizer.GetTextureST(cameraAspect, targetAspect, AspectMode.Fill);
            }

            // Debug.Log($"camera: rotation:{texture.videoRotationAngle} flip:{texture.videoVerticallyMirrored}");
            return _resizer.Resize(texture, w, h, false, mtx, uvRect);
        }

        private static bool IsPortrait(WebCamTexture texture)
        {
            return texture.videoRotationAngle == 90 || texture.videoRotationAngle == 270;
        }

        private static int RoundToEven(float n)
        {
            return Mathf.RoundToInt(n / 2) * 2;
        }
    }
}
