using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Beem.Video {

    /// <summary>
    /// RewindBtn
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class VideoPlayerRewindBtn : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler {

        private Slider progress;

        private void Awake() {
            progress = GetComponent<Slider>();
        }

        public void OnDrag(PointerEventData eventData) {
            VideoPlayerCallBacks.onRewind?.Invoke(progress.value);
        }


        public void OnBeginDrag(PointerEventData eventData) {
            VideoPlayerCallBacks.onRewindStarted?.Invoke();
        }

        public void OnEndDrag(PointerEventData eventData) {
            VideoPlayerCallBacks.onRewindFinished?.Invoke(progress.value);
        }

        public void OnPointerClick(PointerEventData eventData) {
            VideoPlayerCallBacks.onRewind?.Invoke(progress.value);
        }
    }
}
