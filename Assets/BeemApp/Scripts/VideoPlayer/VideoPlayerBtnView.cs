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

        [Header("Action On Play")]
        [SerializeField]
        private UnityEvent onPlay;
        [Header("Action On Pause")]
        [SerializeField]
        private UnityEvent onPause;

        private void OnEnable() {
            VideoPlayerCallBacks.onPlay += OnPlay;
            VideoPlayerCallBacks.onPause += OnPause;
        }

        private void OnDisable() {
            VideoPlayerCallBacks.onPlay -= OnPlay;
            VideoPlayerCallBacks.onPause -= OnPause;
        }

        private void OnPlay() {
            onPlay?.Invoke();
        }

        private void OnPause() {
            onPause?.Invoke();
        }
    }
}
