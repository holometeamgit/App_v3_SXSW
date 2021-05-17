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
        private VideoPlayer videoPlayer;

        [Header("Views for video")]
        [SerializeField]
        private List<AbstractVideoPlayerView> videoPlayerViews;

        private void OnEnable() {
            VideoPlayerCallBacks.onPlay += OnPlay;
            VideoPlayerCallBacks.onPause += OnPause;
            VideoPlayerCallBacks.onRewind += OnRewind;
        }

        private void OnDisable() {
            VideoPlayerCallBacks.onPlay -= OnPlay;
            VideoPlayerCallBacks.onPause -= OnPause;
            VideoPlayerCallBacks.onRewind -= OnRewind;
        }

        private void Start() {

        }

        private void Init() {
            foreach (AbstractVideoPlayerView view in videoPlayerViews) {
                view.Refresh(videoPlayer);
            }
        }

        private void OnPlay() {
            videoPlayer.Play();
            foreach (AbstractVideoPlayerView view in videoPlayerViews) {
                view.UpdateVideo(videoPlayer);
            }
        }

        private void OnPause() {
            videoPlayer.Pause();
            foreach (AbstractVideoPlayerView view in videoPlayerViews) {
                view.Cancel();
            }
        }

        private void OnRewind(float pct) {
            var frame = videoPlayer.frameCount * pct;
            videoPlayer.frame = (long)frame;
        }



    }
}
