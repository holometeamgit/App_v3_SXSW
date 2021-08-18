using Beem.Extenject.Hologram;
using Beem.Extenject.UI;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;
using Zenject;

namespace Beem.Extenject.Video {

    /// <summary>
    /// Abstract video player view
    /// </summary>
    public abstract class AbstractVideoPlayerView : MonoBehaviour, IShow {

        protected CancellationTokenSource cancelTokenSource;

        protected abstract int delay { get; }
        protected abstract bool condition { get; }

        protected VideoPlayer _videoPlayer;

        private SignalBus _signalBus;

        public double Time {
            get { return _videoPlayer.time; }
        }

        public ulong Duration {
            get { return (ulong)(_videoPlayer.frameCount / _videoPlayer.frameRate); }
        }

        public double NTime {
            get { return Time / Duration; }
        }

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        /// <summary>
        /// Refresh data Video Player 
        /// </summary>
        /// <param name="videoPlayer"></param>
        public abstract void Refresh();

        public async void PlayAsync() {
            cancelTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancelTokenSource.Token;
            try {
                while (!cancellationToken.IsCancellationRequested && condition) {
                    Refresh();
                    await Task.Delay(delay);
                }
            } finally {
                if (cancelTokenSource != null) {
                    cancelTokenSource.Dispose();
                    cancelTokenSource = null;
                }
            }
        }

        private void OnEnable() {
            _signalBus.Subscribe<PlaySignal>(PlayAsync);
            _signalBus.Subscribe<PauseSignal>(Cancel);
            _signalBus.Subscribe<StopSignal>(Cancel);
        }

        protected void OnDisable() {
            _signalBus.Unsubscribe<PlaySignal>(PlayAsync);
            _signalBus.Unsubscribe<PauseSignal>(Cancel);
            _signalBus.Unsubscribe<StopSignal>(Cancel);
            Cancel();
        }

        /// <summary>
        /// Clear Info
        /// </summary>
        public void Cancel() {
            if (cancelTokenSource != null) {
                cancelTokenSource.Cancel();
                cancelTokenSource = null;
            }
        }

        public virtual void Show<T>(T parameter) {
            if (parameter is PrerecordedVideoData) {
                PrerecordedVideoData prerecordedVideoData = parameter as PrerecordedVideoData;
                _videoPlayer = prerecordedVideoData.Player;
            }
        }
    }
}
