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

        protected int delay = 2000;

        public void Refresh(VideoPlayer videoPlayer) {
            playBtn.SetActive(!videoPlayer.isPlaying);
            pauseBtn.SetActive(videoPlayer.isPlaying);
        }
    }
}
