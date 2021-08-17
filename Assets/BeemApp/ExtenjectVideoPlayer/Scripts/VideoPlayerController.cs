using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Beem.Extenject.Video {

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

        private bool isPlaying = default;

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
            if (_videoPlayer != null) {
                foreach (AbstractVideoPlayerView view in _videoPlayerViews) {
                    view.Init(_videoPlayer);
                }
            }
        }

        /// <summary>
        /// Play Video
        /// </summary>

        public void OnPlay() {
            if (_videoPlayer != null) {
                if (!_videoPlayer.isPrepared) {
                    _videoPlayer.prepareCompleted += OnPrepare;
                    _videoPlayer.Prepare();
                } else {
                    OnPrepare(_videoPlayer);
                }
                _videoPlayerBtnViews.Refresh(_videoPlayer);
            }
        }

        /// <summary>
        /// Prepare Video
        /// </summary>
        /// <param name="videoPlayer"></param>

        private void OnPrepare(VideoPlayer videoPlayer) {
            if (_videoPlayer != null) {
                _videoPlayer.prepareCompleted -= OnPrepare;
                _videoPlayer.seekCompleted += OnSeekCompleted;
                playerObjects.SetActive(true);
                _videoPlayerBtnViews.Refresh(_videoPlayer);
                _videoPlayer.Play();
                foreach (AbstractVideoPlayerView view in _videoPlayerViews) {
                    view.PlayAsync();
                }
            }
        }

        /// <summary>
        /// Pause Video
        /// </summary>

        public void OnPause() {
            if (_videoPlayer != null) {
                isPlaying = _videoPlayer.isPlaying;
                _videoPlayer.Pause();
                _videoPlayerBtnViews.Refresh(_videoPlayer);
                foreach (AbstractVideoPlayerView view in _videoPlayerViews) {
                    view.Cancel();
                }
            }
        }

        /// <summary>
        /// Rewind Start
        /// </summary>

        public void OnRewindStarted() {
            OnPause();
        }

        public void OnResume() {
            if (isPlaying) {
                OnPlay();
            }
        }

        private void OnApplicationPause(bool pause) {
            if (!pause) {
                OnResume();
            } else {
                OnPause();
            }
        }

        /// <summary>
        /// Rewind Video
        /// </summary>
        /// <param name="pct"></param>
        public void OnRewind(float pct) {
            if (_videoPlayer != null) {
                if (!_videoPlayer.canSetTime) return;
                if (!_videoPlayer.isPrepared) return;
                _videoPlayer.time = pct * (_videoPlayer.frameCount / _videoPlayer.frameRate);
            }
        }

        /// <summary>
        /// Seek Completed
        /// </summary>
        /// <param name="videoPlayer"></param>

        private void OnSeekCompleted(VideoPlayer videoPlayer) {
            if (_videoPlayer != null) {
                _videoPlayer.sendFrameReadyEvents = true;
                _videoPlayer.seekCompleted -= OnSeekCompleted;
                _videoPlayer.frameReady += OnFrameReady;
            }
        }

        /// <summary>
        /// Frame Ready
        /// </summary>
        /// <param name="videoPlayer"></param>
        /// <param name="frame"></param>

        private void OnFrameReady(VideoPlayer videoPlayer, long frame) {
            if (_videoPlayer != null) {
                _videoPlayer.sendFrameReadyEvents = false;
                _videoPlayer.frameReady -= OnFrameReady;
                foreach (AbstractVideoPlayerView view in _videoPlayerViews) {
                    view.Refresh();
                }
            }
        }

        /// <summary>
        /// Rewind Finished
        /// </summary>
        /// <param name="pct"></param>

        public void OnRewindFinished(float pct) {
            OnRewind(pct);
        }

        /// <summary>
        /// Stop Video
        /// </summary>

        public void OnStop() {
            if (_videoPlayer != null) {
                if (_videoPlayer.isPlaying) {
                    _videoPlayer.Stop();
                }
            }
        }

        private void OnSetVideoPlayer(VideoPlayer videoPlayer) {
            if (videoPlayer != null) {
                OnStop();
                _videoPlayer = videoPlayer;
                OnInit();
                OnPlay();
            }
        }
    }
}
