using Beem.Extenject.UI;
using NatCorder;
using NatCorder.Clocks;
using NatCorder.Inputs;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace Beem.Extenject.Record {
    /// <summary>
    /// Snapshot controller
    /// </summary>
    public class SnapShotController {

        private WindowSignal _windowSignals;

        private WindowController _windowController;

        private Camera[] _cameras;

        private CancellationTokenSource cancelTokenSource;

        public SnapShotController(WindowSignal windowSignals, Camera[] cameras) {
            _windowSignals = windowSignals;
            _cameras = cameras;
        }

        [Inject]
        public void Construct(WindowController windowController) {
            _windowController = windowController;
        }

        /// <summary>
        /// Create SnapShot
        /// </summary>
        public async void CreateSnapShotAsync() {
            Debug.Log("CreateSnapShot");
            Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
            Task.Yield();
            _windowController.OnCalledSignal(_windowSignals, screenshot);
            //ScreenShot screenShot = new ScreenShot(_cameras[0]);
            //screenShot.TakeScreenShot((outputPath) => GetTextureAsync(outputPath, (tex) => _windowController.OnCalledSignal(_windowSignals, tex)));
        }

        private void OnRecordComplete(string outputPath) {
            Debug.Log("OnRecordComplete = " + outputPath);

            GetTextureAsync(outputPath, (tex) => _windowController.OnCalledSignal(_windowSignals, tex));
        }

        private async void GetTextureAsync(string url, Action<Texture2D> onSuccess) {
            cancelTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancelTokenSource.Token;
            try {
                await Task.Yield();
                UnityWebRequest request = UnityWebRequest.Get(url);
                var operation = request.SendWebRequest();
                Debug.Log("GetTextureAsync");
                while (!operation.isDone && !cancellationToken.IsCancellationRequested) {
                    await Task.Yield();
                    Debug.Log("operation progress = " + operation.progress);
                }

                Debug.Log("request.result = " + request.result);
                Debug.Log(DownloadHandlerTexture.GetContent(request));
                if (request.result == UnityWebRequest.Result.Success) {
                    onSuccess?.Invoke(DownloadHandlerTexture.GetContent(request));
                }
            } finally {
                if (cancelTokenSource != null) {
                    cancelTokenSource.Dispose();
                    cancelTokenSource = null;
                }
            }
        }

        /// <summary>
        /// Cancel
        /// </summary>
        public void Cancel() {
            if (cancelTokenSource != null) {
                cancelTokenSource.Cancel();
                cancelTokenSource = null;
            }
        }
    }
}
