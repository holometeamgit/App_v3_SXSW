using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Beem.Extenject.Video {

    /// <summary>
    /// PauseBtn
    /// </summary>
    public class VideoPlayerPauseBtn : MonoBehaviour, IPointerClickHandler {
        private VideoPlayerController _videoPlayerController;

        [Inject]
        public void Construct(VideoPlayerController videoPlayerController) {
            _videoPlayerController = videoPlayerController;
        }

        public void OnPointerClick(PointerEventData eventData) {
            _videoPlayerController.OnPause();
        }
    }
}
