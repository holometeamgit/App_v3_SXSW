using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Beem.Video {

    /// <summary>
    /// PauseBtn
    /// </summary>
    public class VideoPlayerPauseBtn : MonoBehaviour, IPointerDownHandler {

        [SerializeField]
        private UnityEvent onPause;
        public void OnPointerDown(PointerEventData eventData) {
            onPause?.Invoke();
        }
    }
}
