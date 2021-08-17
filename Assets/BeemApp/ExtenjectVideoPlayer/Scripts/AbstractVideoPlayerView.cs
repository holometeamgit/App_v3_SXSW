using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

namespace Beem.Extenject.Video {

    /// <summary>
    /// Abstract video player view
    /// </summary>
    public abstract class AbstractVideoPlayerView : MonoBehaviour {

        protected CancellationTokenSource cancelTokenSource;

        protected abstract int delay { get; }
        protected abstract bool condition { get; }

        protected VideoPlayer _videoPlayer;

        public double Time {
            get { return _videoPlayer.time; }
        }

        public ulong Duration {
            get { return (ulong)(_videoPlayer.frameCount / _videoPlayer.frameRate); }
        }

        public double NTime {
            get { return Time / Duration; }
        }

        public virtual void Init(VideoPlayer videoPlayer) {
            _videoPlayer = videoPlayer;
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

        protected void OnDisable() {
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
