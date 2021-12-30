using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Zenject;

namespace Beem.Extenject.Video {

    /// <summary>
    /// PlayBtn
    /// </summary>
    public class VideoPlayerPlayBtn : MonoBehaviour, IPointerClickHandler {

        private VideoPlayerController _videoPlayerController;

        [Inject]
        public void Construct(VideoPlayerController videoPlayerController) {
            _videoPlayerController = videoPlayerController;
        }

        public void OnPointerClick(PointerEventData eventData) {
            _videoPlayerController.OnPlay();
        }

    }
}
