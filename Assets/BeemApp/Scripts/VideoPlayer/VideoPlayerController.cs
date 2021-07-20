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

        private void OnPlay() {
            if (_videoPlayer != null) {
                _videoPlayer.Play();
                _videoPlayerBtnViews.Refresh(_videoPlayer);
                foreach (AbstractVideoPlayerView view in _videoPlayerViews) {
                    view.UpdateVideo(_videoPlayer);
                }
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
                    view.Refresh(_videoPlayer);
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
                OnPlay();
            }
        }

    }
}
