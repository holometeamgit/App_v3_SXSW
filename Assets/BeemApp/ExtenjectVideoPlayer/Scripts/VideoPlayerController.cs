using Beem.Extenject.Hologram;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;
using Zenject;

namespace Beem.Extenject.Video {

    /// <summary>
    /// CurrentPlayer for video
    /// </summary>
    public class VideoPlayerController : ILateDisposable {

        private VideoPlayer _videoPlayer;

        public VideoPlayer Player {
            get {
                return _videoPlayer;
            }
        }

        private double currentTime = default;
        private CancellationTokenSource cancelTokenSource;

        public event Action onPlay = delegate { };
        public event Action onStop = delegate { };
        public event Action onPause = delegate { };
        public event Action<float> onRewind = delegate { };
        public event Action onStartRewind = delegate { };
        public event Action<float> onFinishRewind = delegate { };

        /// <summary>
        /// Play Video
        /// </summary>
        public void OnPlay() {
            if (_videoPlayer == null) {
                return;
            }


            if (!_videoPlayer.isPrepared) {
                _videoPlayer.prepareCompleted += OnPrepare;
                _videoPlayer.Prepare();
            } else {
                OnPrepare(_videoPlayer);
            }


            onPlay?.Invoke();

        }

        /// <summary>
        /// Prepare Video
        /// </summary>
        /// <param name="videoPlayer"></param>

        private void OnPrepare(VideoPlayer videoPlayer) {

            if (_videoPlayer == null) {
                return;
            }

            _videoPlayer.prepareCompleted -= OnPrepare;
            _videoPlayer.seekCompleted += OnSeekCompleted;
            _videoPlayer.Play();
        }

        /// <summary>
        /// Pause Video
        /// </summary>

        public void OnPause() {

            if (_videoPlayer == null) {
                return;
            }

            _videoPlayer.Pause();

            onPause?.Invoke();
        }

        /// <summary>
        /// Rewind Start
        /// </summary>

        public void OnRewindStarted() {
            OnPause();
            onStartRewind?.Invoke();
        }

        /// <summary>
        /// Resume Video
        /// </summary>
        public void OnResume() {

            if (_videoPlayer == null) {
                return;
            }
            if (_videoPlayer.isPaused) {
                OnPlay();
            }
        }

        /// <summary>
        /// Rewind Video
        /// </summary>
        /// <param name="pct"></param>
        public void OnRewind(float percent) {

            if (_videoPlayer == null) {
                return;
            }

            if (!_videoPlayer.canSetTime) return;
            if (!_videoPlayer.isPrepared) return;
            currentTime = percent * (_videoPlayer.frameCount / _videoPlayer.frameRate);
            _videoPlayer.time = currentTime;

            onRewind?.Invoke(percent);
        }

        /// <summary>
        /// Seek Completed
        /// </summary>
        /// <param name="videoPlayer"></param>

        private void OnSeekCompleted(VideoPlayer videoPlayer) {
            if (_videoPlayer == null) {
                return;
            }

            _videoPlayer.sendFrameReadyEvents = true;
            _videoPlayer.seekCompleted -= OnSeekCompleted;
            _videoPlayer.frameReady += OnFrameReady;

        }

        /// <summary>
        /// Frame Ready
        /// </summary>
        /// <param name="videoPlayer"></param>
        /// <param name="frame"></param>

        private void OnFrameReady(VideoPlayer videoPlayer, long frame) {
            if (_videoPlayer == null) {
                return;
            }

            _videoPlayer.sendFrameReadyEvents = false;
            _videoPlayer.frameReady -= OnFrameReady;
        }

        private async void LoadVideoFrameComletedAsync() {

            if (_videoPlayer == null) {
                return;
            }

            Cancel();

            cancelTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancelTokenSource.Token;

            try {
                while (!cancellationToken.IsCancellationRequested && Mathf.Abs((float)(currentTime - _videoPlayer.time)) > 1f) {
                    await Task.Yield();
                }

                if (!cancellationToken.IsCancellationRequested) {
                    OnResume();
                }
            } finally {
                if (cancelTokenSource != null) {
                    cancelTokenSource.Dispose();
                    cancelTokenSource = null;
                }
            }
        }

        /// <summary>
        /// Clear Info
        /// </summary>
        private void Cancel() {
            if (cancelTokenSource != null) {
                cancelTokenSource.Cancel();
                cancelTokenSource = null;
            }
        }

        /// <summary>
        /// Rewind Finished
        /// </summary>
        /// <param name="pct"></param>

        public void OnRewindFinished(float percent) {
            OnRewind(percent);
            LoadVideoFrameComletedAsync();
            onFinishRewind?.Invoke(percent);
        }

        /// <summary>
        /// Stop Video
        /// </summary>

        public void OnStop() {
            if (_videoPlayer == null) {
                return;
            }

            if (_videoPlayer.isPlaying) {
                _videoPlayer.Stop();
            }
            Cancel();
            onStop?.Invoke();
        }

        /// <summary>
        /// VideoPlayer setup
        /// </summary>
        /// <param name="videoPlayer"></param>
        public void SetVideoPlayer(VideoPlayer videoPlayer) {

            if (videoPlayer == null) {
                return;
            }


            OnStop();

            _videoPlayer = videoPlayer;

            OnPlay();

        }

        public void LateDispose() {
            OnStop();
        }
    }
}
