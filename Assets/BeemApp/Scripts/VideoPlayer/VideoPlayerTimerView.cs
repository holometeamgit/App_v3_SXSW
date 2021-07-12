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
            if (videoPlayer == null || !videoPlayer.isPrepared) {
                Cancel();
                return;
            }

            TimeSpan timeSpan = TimeSpan.FromSeconds(videoPlayer.frame / videoPlayer.frameRate);

            if (timerText != null) {
                if (timeSpan.TotalHours > 0) {
                    timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                } else {
                    timerText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
                }
            }
        }
    }
}
