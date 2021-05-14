using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Video;

namespace Beem.Video {

    public class VideoPlayerPauseBtn : MonoBehaviour, IPointerDownHandler {
        [SerializeField]
        private VideoPlayer videoPlayer;

        public void OnPointerDown(PointerEventData eventData) {
            videoPlayer.Pause();
        }
    }
}
