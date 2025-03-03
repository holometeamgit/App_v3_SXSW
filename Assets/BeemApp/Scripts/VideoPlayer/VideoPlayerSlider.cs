﻿using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Beem.Video {

    /// <summary>
    /// Progress bar
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class VideoPlayerSlider : AbstractVideoPlayerView {

        private Slider progress;

        protected override int delay => 1000;

        protected override bool condition => true;

        private void Awake() {
            progress = GetComponent<Slider>();
        }

        public override void Init(VideoPlayer videoPlayer) {
            base.Init(videoPlayer);
            if (progress == null) {
                progress = GetComponent<Slider>();
            }
            progress.value = 0f;
        }

        public override void Refresh() {

            if (_videoPlayer == null || !_videoPlayer.isPrepared) {
                return;
            }

            if (_videoPlayer.frameCount > 0) {
                progress.value = (float)NTime;
            }
        }
    }
}
