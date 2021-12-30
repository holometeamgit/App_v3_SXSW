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
            progress.value = 0f;
        }

        public override void Refresh() {
            if (Player == null || !Player.isPrepared) {
                return;
            }

            if (Player.frameCount > 0) {
                progress.value = (float)NTime;
            }
        }
    }
}
