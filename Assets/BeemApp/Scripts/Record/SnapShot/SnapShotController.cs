using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Beem.Record.SnapShot {
    /// <summary>
    /// Snapshot controller
    /// </summary>
    public class SnapShotController : MonoBehaviour {

        private CancellationTokenSource cancelTokenSource;

        private void OnEnable() {
            SnapShotCallBacks.onSnapshotStarted += CreateSnapShot;
        }

        private void OnDisable() {
            SnapShotCallBacks.onSnapshotStarted -= CreateSnapShot;
        }

        private async void CreateSnapShot() {
            cancelTokenSource = new CancellationTokenSource();
            try {
                Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture(1);
                await Task.Yield();
                SnapShotCallBacks.onSnapshotEnded?.Invoke(screenshot);
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
