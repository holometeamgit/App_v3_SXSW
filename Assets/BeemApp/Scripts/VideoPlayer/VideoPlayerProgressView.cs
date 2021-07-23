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
    /// Progress bar View
    /// </summary>
    public class VideoPlayerProgressView : AbstractVideoPlayerView {

        [Header("Progress Bar")]
        [SerializeField]
        private Image progress;

        [Header("Progress Handle")]
        [SerializeField]
        private Transform handle;

        protected override int delay => 1000;

        protected override bool condition => true;

        private void Awake() {
            progress.fillAmount = 0f;
        }

        public override void Refresh() {

            if (_videoPlayer == null || !_videoPlayer.isPrepared) {
                return;
            }

            if (_videoPlayer.frameCount > 0) {
                float pct = (float)_videoPlayer.frame / (float)_videoPlayer.frameCount;
                progress.fillAmount = pct;
                Vector3 pos = handle.localPosition;
                pos.x = (progress.rectTransform.rect.xMax - progress.rectTransform.rect.xMin) * pct + progress.rectTransform.rect.xMin;
                handle.localPosition = pos;
            }
        }
    }
}
