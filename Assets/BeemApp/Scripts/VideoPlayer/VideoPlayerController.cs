using System.Collections;
using System.Collections.Generic;
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

        private bool isPlaying = default;

        private void OnEnable() {
            VideoPlayerCallBacks.onPlay += OnPlay;
            VideoPlayerCallBacks.onPause += OnPause;
            VideoPlayerCallBacks.onRewindStarted += OnRewindStarted;
            VideoPlayerCallBacks.onRewind += OnRewind;
            VideoPlayerCallBacks.onRewindFinished += OnRewindFinished;
            VideoPlayerCallBacks.onSetVideoPlayer += OnSetVideoPlayer;
            foreach (VideoPlayer item in FindObjectsOfType<VideoPlayer>()) {
                if (item.gameObject.name == "VideoQuad") {
                    OnSetVideoPlayer(item);
                    break;
                }
            }
        }

        private void OnDisable() {
            VideoPlayerCallBacks.onPlay -= OnPlay;
            VideoPlayerCallBacks.onPause -= OnPause;
            VideoPlayerCallBacks.onRewindStarted -= OnRewindStarted;
            VideoPlayerCallBacks.onRewind -= OnRewind;
            VideoPlayerCallBacks.onRewindFinished -= OnRewindFinished;
            VideoPlayerCallBacks.onSetVideoPlayer -= OnSetVideoPlayer;
            OnStop();
        }

        private void OnInit() {
            if (_videoPlayer != null) {
                _videoPlayer.Prepare();
                foreach (AbstractVideoPlayerView view in _videoPlayerViews) {
                    view.Init(_videoPlayer);
                }
            }
        }

        private void OnPlay() {
            if (_videoPlayer != null) {
                _videoPlayer.Play();
                if (_videoPlayer.isPrepared) {
                    _videoPlayer.prepareCompleted -= OnPrepare;
                    OnPrepare(_videoPlayer);
                } else {
                    _videoPlayer.prepareCompleted += OnPrepare;
                }
            }
        }

        private void OnPrepare(VideoPlayer videoPlayer) {
            playerObjects.SetActive(true);
            _videoPlayerBtnViews.Refresh(videoPlayer);
            foreach (AbstractVideoPlayerView view in _videoPlayerViews) {
                view.Play();
            }
        }

        private void OnPause() {
            if (_videoPlayer != null) {
                _videoPlayer.Pause();
                _videoPlayerBtnViews.Refresh(_videoPlayer);
                foreach (AbstractVideoPlayerView view in _videoPlayerViews) {
                    view.Cancel();
                }
            }
        }

        private void OnRewindStarted() {
            if (_videoPlayer != null) {
                isPlaying = _videoPlayer.isPlaying;
            }
            OnPause();
        }

        private void OnRewind(float pct) {
            if (_videoPlayer != null) {
                if (!_videoPlayer.canSetTime) return;
                if (!_videoPlayer.isPrepared) return;
                _videoPlayer.time = pct * (_videoPlayer.frameCount / _videoPlayer.frameRate);
            }
        }

        private void OnRewindFinished(float pct) {
            OnRewind(pct);
            if (isPlaying) {
                OnPlay();
            }
        }

        private void OnStop() {
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
