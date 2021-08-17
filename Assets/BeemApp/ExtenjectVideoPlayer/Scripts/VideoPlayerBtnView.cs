using UnityEngine;
using UnityEngine.Video;

namespace Beem.Extenject.Video {

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

        public void Refresh(VideoPlayer videoPlayer) {
            playBtn.SetActive(!videoPlayer.isPlaying);
            pauseBtn.SetActive(videoPlayer.isPlaying);
        }
    }
}
