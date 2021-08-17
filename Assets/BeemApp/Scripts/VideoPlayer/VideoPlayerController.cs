using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

namespace Beem.Video {

    /// <summary>
    /// CurrentPlayer for video
    /// </summary>
    public class VideoPlayerController : MonoBehaviour {

        [Header("Video Player")]
        [SerializeField]
        private VideoPlayer _videoPlayer;

        [Header("Views for video")]
        [SerializeField]
        private List<AbstractVideoPlayerView> _videoPlayerViews;

        [SerializeField]
        private VideoPlayerBtnView _videoPlayerBtnViews;

        [SerializeField]
        private GameObject playerObjects;

        public static Action<VideoPlayer> onSetVideoPlayer;

        private double currentTime = default;
        private CancellationTokenSource cancelTokenSource;

        private void OnEnable() {
            onSetVideoPlayer += OnSetVideoPlayer;
            foreach (VideoPlayer item in FindObjectsOfType<VideoPlayer>()) {
                if (item.gameObject.name == "VideoQuad") {
                    OnSetVideoPlayer(item);
                    break;
                }
            }
        }

        private void OnDisable() {
            onSetVideoPlayer -= OnSetVideoPlayer;
            OnStop();
        }

        private void OnInit() {

            if (_videoPlayer == null) {
                return;
            }

            foreach (AbstractVideoPlayerView view in _videoPlayerViews) {
                view.Init(_videoPlayer);
            }
        }

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
            playerObjects.SetActive(true);
            _videoPlayerBtnViews.Refresh(true);
            _videoPlayer.Play();
            foreach (AbstractVideoPlayerView view in _videoPlayerViews) {
                view.PlayAsync();
            }

        }

        /// <summary>
        /// Pause Video
        /// </summary>

        public void OnPause() {

            if (_videoPlayer == null) {
                return;
            }

            _videoPlayer.Pause();
            _videoPlayerBtnViews.Refresh(false);
            foreach (AbstractVideoPlayerView view in _videoPlayerViews) {
                view.Cancel();
            }

        }

        /// <summary>
        /// Rewind Start
        /// </summary>

        public void OnRewindStarted() {
            OnPause();
        }

        public void OnResume() {

            if (_videoPlayer == null) {
                return;
            }
            if (_videoPlayer.isPaused) {
                OnPlay();
            }
        }

        private void OnApplicationPause(bool pause) {
            if (pause) {
                OnPause();
            }
        }

        private void OnApplicationFocus(bool focus) {
            if (focus) {
                OnResume();
            }
        }

        /// <summary>
        /// Rewind Video
        /// </summary>
        /// <param name="pct"></param>
        public void OnRewind(float pct) {

            if (_videoPlayer == null) {
                return;
            }

            if (!_videoPlayer.canSetTime) return;
            if (!_videoPlayer.isPrepared) return;
            currentTime = pct * (_videoPlayer.frameCount / _videoPlayer.frameRate);
            _videoPlayer.time = currentTime;
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
        public void Cancel() {
            if (cancelTokenSource != null) {
                cancelTokenSource.Cancel();
                cancelTokenSource = null;
            }
        }

        /// <summary>
        /// Rewind Finished
        /// </summary>
        /// <param name="pct"></param>

        public void OnRewindFinished(float pct) {
            OnRewind(pct);
            LoadVideoFrameComletedAsync();
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
        }

        private void OnSetVideoPlayer(VideoPlayer videoPlayer) {

            if (videoPlayer == null) {
                return;
            }

            OnStop();
            _videoPlayer = videoPlayer;
            OnInit();
            OnPlay();
        }
    }
}
