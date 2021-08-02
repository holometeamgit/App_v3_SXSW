using Beem.Extenject.UI;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Record {
    /// <summary>
    /// Snapshot controller
    /// </summary>
    public class SnapShotController {

        private WindowSignal _windowSignals;

        private WindowController _windowController;

        private CancellationTokenSource cancelTokenSource;

        private SignalBus _signalBus;

        private Camera _cameras;

        private bool takeScreenshotOnNextFrame;

        public SnapShotController(WindowSignal windowSignals, Camera cameras) {
            _windowSignals = windowSignals;
            _cameras = cameras;
        }

        [Inject]
        public void Construct(SignalBus signalBus, WindowController windowController) {
            _windowController = windowController;
            _signalBus = signalBus;
        }

        /// <summary>
        /// Create SnapShot
        /// </summary>
        public async void CreateSnapShotAsync() {
            cancelTokenSource = new CancellationTokenSource();
            try {
                _signalBus.Fire(new ViewSignal(false));
                await Task.Yield();
                //_cameras.targetTexture = RenderTexture.GetTemporary(Mathf.FloorToInt(Screen.width / 2f), Mathf.FloorToInt(Screen.height / 2f), 16);
                //ScreenShot screenShot = new ScreenShot(_cameras);
                //screenShot.TakeScreenShot();
                //await Task.Yield();
                Texture2D screenshot = await ToTexture2D();
                await Task.Yield();
                _signalBus.Fire(new ViewSignal(true));
                _windowController.OnCalledSignal(_windowSignals, screenshot);
            } finally {
                if (cancelTokenSource != null) {
                    cancelTokenSource.Dispose();
                    cancelTokenSource = null;
                }
            }
        }

        private async Task<Texture2D> ToTexture2D(RenderTexture rTex) {
            Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
            var old_rt = RenderTexture.active;
            RenderTexture.active = rTex;

            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();
            Debug.Log("Apply");
            RenderTexture.active = old_rt;
            return tex;
        }

        private async Task<Texture2D> ToTexture2D() {
            return ScreenCapture.CaptureScreenshotAsTexture();
        }

        public void Cancel() {
            if (cancelTokenSource != null) {
                cancelTokenSource.Cancel();
                cancelTokenSource = null;
            }
        }
    }
}
