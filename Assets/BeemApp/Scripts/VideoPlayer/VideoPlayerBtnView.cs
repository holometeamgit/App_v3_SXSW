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

        public void Refresh(bool isPlaying) {
            playBtn.SetActive(!isPlaying);
            pauseBtn.SetActive(isPlaying);
        }
    }
}
