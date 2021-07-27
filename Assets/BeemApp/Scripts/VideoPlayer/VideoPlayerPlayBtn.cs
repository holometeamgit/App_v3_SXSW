using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Beem.Video {

    /// <summary>
    /// PlayBtn
    /// </summary>
    public class VideoPlayerPlayBtn : MonoBehaviour, IPointerDownHandler {

        public void OnPointerDown(PointerEventData eventData) {
            VideoPlayerCallBacks.onPlay?.Invoke();
        }
    }
}
