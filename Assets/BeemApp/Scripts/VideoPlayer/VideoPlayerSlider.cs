using System.Collections;
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

        private void Awake() {
            progress = GetComponent<Slider>();
        }

        public override void Refresh(VideoPlayer videoPlayer) {

            if (videoPlayer == null || !videoPlayer.isPrepared) {
                progress.value = 0f;
                return;
            }

            if (videoPlayer.frameCount > 0) {
                progress.value = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
            }
        }
    }
}
