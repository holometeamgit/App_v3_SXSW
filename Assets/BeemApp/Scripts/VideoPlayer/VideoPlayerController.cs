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
            HelperFunctions.DevLogError("OnEnable");
            VideoPlayerCallBacks.onPlay += OnPlay;
            VideoPlayerCallBacks.onPause += OnPause;
            VideoPlayerCallBacks.onRewind += OnRewind;
            VideoPlayerCallBacks.onSetVideoPlayer += OnSetVideoPlayer;
            foreach (VideoPlayer item in FindObjectsOfType<VideoPlayer>()) {
                if (item.gameObject.name == "VideoQuad") {
                    HelperFunctions.DevLogError("FoundInEnable");
                    OnSetVideoPlayer(item);
                    break;
                }
            }
            HelperFunctions.DevLogError("OnEnableEnd");
        }

        private void OnDisable() {
            VideoPlayerCallBacks.onPlay -= OnPlay;
            VideoPlayerCallBacks.onPause -= OnPause;
            VideoPlayerCallBacks.onRewind -= OnRewind;
            VideoPlayerCallBacks.onSetVideoPlayer -= OnSetVideoPlayer;
            OnStop();
        }

        private void Init() {
            HelperFunctions.DevLogError("Init");
            if (_videoPlayer != null) {
                HelperFunctions.DevLogError("_videoPlayer = " + _videoPlayer);
                foreach (AbstractVideoPlayerView view in _videoPlayerViews) {
                    HelperFunctions.DevLogError("view = " + view.gameObject.name);
                    view.Refresh(_videoPlayer);
                }
            }
            HelperFunctions.DevLogError("EndInit");
        }

        private void OnPlay() {
            if (_videoPlayer != null) {
                _videoPlayer.Play();
                foreach (AbstractVideoPlayerView view in _videoPlayerViews) {
                    view.UpdateVideo(_videoPlayer);
                }
            }
            HelperFunctions.DevLogError("OnPlay");
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

        private void OnStop() {
            if (_videoPlayer != null) {
                if (_videoPlayer.isPlaying) {
                    _videoPlayer.Stop();
                    /*
                    foreach (AbstractVideoPlayerView view in _videoPlayerViews) {
                        view.Cancel();
                    }
                    */
                }
            }
        }

        private void OnSetVideoPlayer(VideoPlayer videoPlayer) {
            if (videoPlayer != null) {
                HelperFunctions.DevLogError("videoPlayer = " + videoPlayer);
                OnStop();
                HelperFunctions.DevLogError("Test1");
                _videoPlayer = videoPlayer;
                HelperFunctions.DevLogError("Test2");
                //Init();
                //HelperFunctions.DevLogError("Test3");
                VideoPlayerCallBacks.onPlay?.Invoke();
                HelperFunctions.DevLogError("OnSetVideoPlayer");
            }
        }

    }
}
