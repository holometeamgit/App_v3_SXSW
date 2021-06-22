using UnityEngine;
using UnityEngine.EventSystems;

namespace Beem.Video {

    /// <summary>
    /// PauseBtn
    /// </summary>
    public class VideoPlayerPauseBtn : MonoBehaviour, IPointerDownHandler {
        public void OnPointerDown(PointerEventData eventData) {
            VideoPlayerCallBacks.onPause?.Invoke();
        }
    }
}
