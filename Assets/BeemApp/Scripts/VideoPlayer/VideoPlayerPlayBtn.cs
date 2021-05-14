using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Video;
namespace Beem.Video {

    /// <summary>
    /// PlayBtn
    /// </summary>
    public class VideoPlayerPlayBtn : MonoBehaviour, IPointerDownHandler {
        [Header("Action On VideoPlayer Button Click")]
        [SerializeField]
        private UnityEvent onClick;

        public void OnPointerDown(PointerEventData eventData) {
            onClick?.Invoke();
        }
    }
}
