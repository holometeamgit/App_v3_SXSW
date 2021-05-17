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

        [Header("Timer for video")]
        [SerializeField]
        private VideoPlayerTimerView videoPlayerTimerView;

        [Header("Progress bar for video")]
        [SerializeField]
        private VideoPlayerProgressBar videoPlayerProgressBar;

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

        private void OnPlay() {
            videoPlayer.Play();
            videoPlayerTimerView.UpdateTimer(videoPlayer);
            videoPlayerProgressBar.UpdateProgressBar(videoPlayer);
        }

        private void OnPause() {
            videoPlayer.Pause();
            videoPlayerTimerView.Clear();
            videoPlayerProgressBar.Clear();
        }

        private void OnRewind(float pct) {
            var frame = videoPlayer.frameCount * pct;
            videoPlayer.frame = (long)frame;
        }



    }
}
