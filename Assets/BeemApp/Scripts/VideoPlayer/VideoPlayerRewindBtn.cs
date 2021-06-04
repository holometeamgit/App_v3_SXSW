using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Beem.Video {

    /// <summary>
    /// RewindBtn
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class VideoPlayerRewindBtn : MonoBehaviour, IDragHandler, IPointerDownHandler {

        private Slider progress;

        private void Awake() {
            progress = GetComponent<Slider>();
        }

        private void SkipToPercent(float pct) {
            VideoPlayerCallBacks.onRewind?.Invoke(pct);
        }

        public void OnPointerDown(PointerEventData eventData) {
            SkipToPercent(progress.value);
        }

        public void OnDrag(PointerEventData eventData) {
            SkipToPercent(progress.value);
        }
    }
}
