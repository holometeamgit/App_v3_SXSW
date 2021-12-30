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
    public abstract class AbstractVideoPlayerView : MonoBehaviour {

        protected CancellationTokenSource cancelTokenSource;

        protected abstract int delay { get; }
        protected abstract bool condition { get; }

        protected VideoPlayer Player {
            get {
                return _videoPlayerController.Player;
            }
        }

        private VideoPlayerController _videoPlayerController;

        public double Time {
            get { return Player.time; }
        }

        public ulong Duration {
            get { return (ulong)(Player.frameCount / Player.frameRate); }
        }

        public double NTime {
            get { return Time / Duration; }
        }

        [Inject]
        public void Construct(VideoPlayerController videoPlayerController) {
            _videoPlayerController = videoPlayerController;
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
            _videoPlayerController.onStop += Cancel;
            _videoPlayerController.onPause += Cancel;
            _videoPlayerController.onPlay += PlayAsync;
            if (Player.isPlaying) {
                PlayAsync();
            }
        }

        protected void OnDisable() {
            _videoPlayerController.onStop -= Cancel;
            _videoPlayerController.onPause -= Cancel;
            _videoPlayerController.onPlay -= PlayAsync;
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
