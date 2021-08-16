using Beem.Extenject.UI;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Record {
    /// <summary>
    /// Snapshot controller
    /// </summary>
    public class SnapShotController : IInitializable, ILateDisposable {

        private SignalBus _signalBus;

        private CancellationTokenSource cancelTokenSource;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        /// <summary>
        /// Create SnapShot
        /// </summary>
        public async void CreateSnapShotAsync() {
            Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
            await Task.Yield();
            _signalBus.Fire(new SnapShotEndSignal());
            _signalBus.Fire(new SnapShotFinishSignal(screenshot));
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

        public void Initialize() {
            _signalBus.Subscribe<SnapShotStartSignal>(CreateSnapShotAsync);
        }

        public void LateDispose() {
            _signalBus.Unsubscribe<SnapShotStartSignal>(CreateSnapShotAsync);
        }
    }
}
