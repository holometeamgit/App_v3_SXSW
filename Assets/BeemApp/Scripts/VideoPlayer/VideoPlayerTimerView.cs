using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Beem.Video {

    /// <summary>
    /// Timer View
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class VideoPlayerTimerView : AbstractVideoPlayerView {

        private Text timerText;

        private void Awake() {
            timerText = GetComponent<Text>();
        }

        public override void Refresh(VideoPlayer videoPlayer) {
            TimeSpan timeSpan = TimeSpan.FromSeconds(videoPlayer.frame);
            if (timerText != null) {
                timerText.text = string.Concat(timeSpan);
            }
        }
    }
}
