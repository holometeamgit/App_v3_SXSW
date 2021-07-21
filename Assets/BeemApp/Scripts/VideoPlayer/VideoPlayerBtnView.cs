using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace Beem.Video {

    /// <summary>
    /// Video Player view
    /// </summary>
    public class VideoPlayerBtnView : MonoBehaviour {

        [Header("Action On Play")]
        [SerializeField]
        private GameObject playBtn;
        [Header("Action On Pause")]
        [SerializeField]
        private GameObject pauseBtn;

        protected CancellationTokenSource cancelTokenSource;
        protected int delay = 2000;

        public async void Refresh(VideoPlayer videoPlayer) {
            cancelTokenSource = new CancellationTokenSource();
            playBtn.SetActive(!videoPlayer.isPlaying);
            pauseBtn.SetActive(videoPlayer.isPlaying);
            await Task.Delay(delay);
            playBtn.SetActive(!videoPlayer.isPlaying);
            pauseBtn.SetActive(videoPlayer.isPlaying);
        }

        protected void OnDisable() {
            Cancel();
        }

        /// <summary>
        /// Clear Info
        /// </summary>
        public void Cancel() {
            if (cancelTokenSource != null) {
                cancelTokenSource.Cancel();
                cancelTokenSource = null;
            }
        }
    }
}
