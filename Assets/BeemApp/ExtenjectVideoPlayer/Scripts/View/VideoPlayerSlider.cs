using Beem.Extenject.Hologram;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Beem.Extenject.Video {

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

        public override void Show<T>(T parameter) {
            base.Show(parameter);
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
