using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

namespace Beem.Video {

    /// <summary>
    /// Abstract video player view
    /// </summary>
    public abstract class AbstractVideoPlayerView : MonoBehaviour {

        protected CancellationTokenSource cancelTokenSource;

        private const int DELAY = 1000;

        /// <summary>
        /// Refresh data Video Player 
        /// </summary>
        /// <param name="videoPlayer"></param>
        public abstract void Refresh(VideoPlayer videoPlayer);

        public async void UpdateVideo(VideoPlayer videoPlayer) {
            cancelTokenSource = new CancellationTokenSource();
            try {
                while (true) {
                    Refresh(videoPlayer);
                    await Task.Delay(DELAY);
                }
            } finally {
                if (cancelTokenSource != null) {
                    cancelTokenSource.Dispose();
                    cancelTokenSource = null;
                }
            }
        }

        protected void OnDestroy() {
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
