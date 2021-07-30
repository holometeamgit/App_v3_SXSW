using Beem.Extenject.UI;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Beem.Record.SnapShot {
    /// <summary>
    /// Snapshot controller
    /// </summary>
    public class SnapShotController : MonoBehaviour {

        [SerializeField]
        private WindowSignal _windowSignals;

        private WindowController _windowController;

        private CancellationTokenSource cancelTokenSource;

        [Inject]
        public void Construct(WindowController windowController) {
            _windowController = windowController;
        }

        public async void CreateSnapShotAsync() {
            cancelTokenSource = new CancellationTokenSource();
            try {
                Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture(1);
                await Task.Yield();
                _windowController.OnCalledSignal(_windowSignals, screenshot);
            } finally {
                if (cancelTokenSource != null) {
                    cancelTokenSource.Dispose();
                    cancelTokenSource = null;
                }
            }
        }

        public void Cancel() {
            if (cancelTokenSource != null) {
                cancelTokenSource.Cancel();
                cancelTokenSource = null;
            }
        }
    }
}
