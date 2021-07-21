﻿using System.Collections;
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

        protected abstract int delay { get; }
        protected abstract bool condition { get; }

        protected VideoPlayer _videoPlayer;

        public virtual void Init(VideoPlayer videoPlayer) {
            _videoPlayer = videoPlayer;
        }

        /// <summary>
        /// Refresh data Video Player 
        /// </summary>
        /// <param name="videoPlayer"></param>
        public abstract void Refresh();

        public async void UpdateVideo() {
            cancelTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancelTokenSource.Token;
            try {
                await Task.Delay(delay);
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
