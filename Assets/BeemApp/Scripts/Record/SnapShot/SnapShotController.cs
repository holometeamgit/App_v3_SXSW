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
            Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
            await Task.Yield();
            _windowController.OnCalledSignal(_windowSignals, screenshot);
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
