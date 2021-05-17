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
    [RequireComponent(typeof(Image))]
    public class VideoPlayerProgressBar : AbstractVideoPlayerView {

        private Image progress;

        private void Awake() {
            progress = GetComponent<Image>();
        }

        public override void Refresh(VideoPlayer videoPlayer) {
            if (videoPlayer.frameCount > 0) {
                progress.fillAmount = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
            }
        }
    }
}
