using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;
using Zenject;

namespace Beem.Extenject.Video {

    /// <summary>
    /// Abstract video player view
    /// </summary>
    public abstract class AbstractVideoPlayerView : MonoBehaviour {

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

        public virtual void OnInit(InitSignal initSignal) {
            _videoPlayer = initSignal.Player;
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
            _signalBus.Subscribe<InitSignal>(OnInit);
            _signalBus.Subscribe<PlaySignal>(PlayAsync);
            _signalBus.Subscribe<PauseSignal>(Cancel);
            _signalBus.Subscribe<StopSignal>(Cancel);
        }

        protected void OnDisable() {
            _signalBus.Unsubscribe<InitSignal>(OnInit);
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
    }
}
