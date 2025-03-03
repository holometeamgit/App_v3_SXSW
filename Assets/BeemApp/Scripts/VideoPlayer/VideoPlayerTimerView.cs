﻿using System;
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

        protected override int delay => 1000;

        protected override bool condition => true;

        private void Awake() {
            timerText = GetComponent<Text>();
            timerText.text = string.Empty;
        }

        public override void Init(VideoPlayer videoPlayer) {
            base.Init(videoPlayer);
            if (timerText == null) {
                timerText = GetComponent<Text>();
            }
            timerText.text = string.Empty;
        }

        public override void Refresh() {
            if (_videoPlayer == null || !_videoPlayer.isPrepared) {
                return;
            }

            if (_videoPlayer.frameCount > 0) {
                TimeSpan timeSpan = TimeSpan.FromSeconds((float)Time);

                if (timerText != null) {
                    if (timeSpan.Hours > 0) {
                        timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                    } else {
                        timerText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
                    }
                }
            }
        }
    }
}
