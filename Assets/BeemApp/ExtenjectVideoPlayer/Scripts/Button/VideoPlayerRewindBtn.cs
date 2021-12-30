using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Beem.Extenject.Video {

    /// <summary>
    /// RewindBtn
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class VideoPlayerRewindBtn : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {

        private Slider _progress;
        private VideoPlayerController _videoPlayerController;

        [Inject]
        public void Construct(VideoPlayerController videoPlayerController) {
            _videoPlayerController = videoPlayerController;
        }

        private void Awake() {
            _progress = GetComponent<Slider>();
        }

        public void OnDrag(PointerEventData eventData) {
            _videoPlayerController.OnRewind(_progress.value);
        }

        public void OnBeginDrag(PointerEventData eventData) {
            _videoPlayerController.OnRewindStarted();
        }

        public void OnEndDrag(PointerEventData eventData) {
            _videoPlayerController.OnRewindFinished(_progress.value);
        }
    }
}
