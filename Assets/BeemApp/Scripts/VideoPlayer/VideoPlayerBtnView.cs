using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace Beem.Video {

    /// <summary>
    /// Video Player view
    /// </summary>
    public class VideoPlayerBtnView : MonoBehaviour {

        [Header("Video Player")]
        [SerializeField]
        private VideoPlayer videoPlayer;
        [Header("Action On Play")]
        [SerializeField]
        private UnityEvent onPlay;
        [Header("Action On Pause")]
        [SerializeField]
        private UnityEvent onPause;

        private void Start() {
            UpdateVideoStatus();
        }

        /// <summary>
        /// Refresh status
        /// </summary>
        public void UpdateVideoStatus() {
            if (!videoPlayer.isPaused) {
                onPause?.Invoke();
            }
            else {
                onPlay?.Invoke();
            }
        }
    }
}
