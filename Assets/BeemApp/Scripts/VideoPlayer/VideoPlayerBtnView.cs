using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace Beem.Video {

    [System.Serializable]
    public class MyBoolEvent : UnityEvent<bool> {
    }

    public class VideoPlayerBtnView : MonoBehaviour {

        [SerializeField]
        private VideoPlayer videoPlayer;

        [SerializeField]
        private MyBoolEvent onPlay;

        public void UpdateVideoStatus() {
            onPlay?.Invoke(videoPlayer.isPlaying);
        }
    }
}
