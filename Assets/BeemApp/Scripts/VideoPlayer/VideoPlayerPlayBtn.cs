using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Beem.Video {

    /// <summary>
    /// PlayBtn
    /// </summary>
    public class VideoPlayerPlayBtn : MonoBehaviour, IPointerDownHandler {

        [SerializeField]
        private UnityEvent onPlay;
        public void OnPointerDown(PointerEventData eventData) {
            onPlay?.Invoke();
        }
    }
}
