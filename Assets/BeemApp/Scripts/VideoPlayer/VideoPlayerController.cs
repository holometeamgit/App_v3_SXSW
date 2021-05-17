﻿using System.Collections;
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

        private void OnEnable() {
            VideoPlayerCallBacks.onPlay += OnPlay;
            VideoPlayerCallBacks.onPause += OnPause;
            VideoPlayerCallBacks.onRewind += OnRewind;
            VideoPlayerCallBacks.onSetVideoPlayer += OnSetVideoPlayer;
        }

        private void OnDisable() {
            VideoPlayerCallBacks.onPlay -= OnPlay;
            VideoPlayerCallBacks.onPause -= OnPause;
            VideoPlayerCallBacks.onRewind -= OnRewind;
            VideoPlayerCallBacks.onSetVideoPlayer -= OnSetVideoPlayer;
        }

        private void Start() {
            Init();
            Debug.LogError("Start");
        }

        private void Init() {
            if (_videoPlayer != null) {
                foreach (AbstractVideoPlayerView view in _videoPlayerViews) {
                    view.Refresh(_videoPlayer);
                }
            }
        }

        private void OnPlay() {
            if (_videoPlayer != null) {
                _videoPlayer.Play();
                foreach (AbstractVideoPlayerView view in _videoPlayerViews) {
                    view.UpdateVideo(_videoPlayer);
                }
            }
        }

        private void OnPause() {
            if (_videoPlayer != null) {
                _videoPlayer.Pause();
                foreach (AbstractVideoPlayerView view in _videoPlayerViews) {
                    view.Cancel();
                }
            }
        }

        private void OnRewind(float pct) {
            if (_videoPlayer != null) {
                var frame = _videoPlayer.frameCount * pct;
                _videoPlayer.frame = (long)frame;
            }
        }

        private void OnSetVideoPlayer(VideoPlayer videoPlayer) {
            if (_videoPlayer != null) {
                if (_videoPlayer.isPlaying) {
                    _videoPlayer.Stop();
                }
            }
            _videoPlayer = videoPlayer;
            Init();
            Debug.LogError("Init");
        }

    }
}
