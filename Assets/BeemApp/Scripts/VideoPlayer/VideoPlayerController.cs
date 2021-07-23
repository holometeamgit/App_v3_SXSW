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

        private void OnEnable() {
            VideoPlayerCallBacks.onPlay += OnPlay;
            VideoPlayerCallBacks.onPause += OnPause;
            VideoPlayerCallBacks.onRewind += OnRewind;
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
            VideoPlayerCallBacks.onRewind -= OnRewind;
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
                view.UpdateVideo();
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

        private void OnRewind(float pct) {
            if (_videoPlayer != null) {
                var frame = _videoPlayer.frameCount * pct;
                _videoPlayer.frame = (long)frame;
                foreach (AbstractVideoPlayerView view in _videoPlayerViews) {
                    view.Refresh();
                }
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
