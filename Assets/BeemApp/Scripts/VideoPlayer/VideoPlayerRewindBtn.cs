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
    [RequireComponent(typeof(Image))]
    public class VideoPlayerRewindBtn : MonoBehaviour, IDragHandler, IPointerDownHandler {

        private Image progress;

        private void Awake() {
            progress = GetComponent<Image>();
        }

        public void OnDrag(PointerEventData eventData) {
            TrySkip(eventData);
        }

        public void OnPointerDown(PointerEventData eventData) {
            TrySkip(eventData);
        }

        private void TrySkip(PointerEventData eventData) {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(progress.rectTransform, eventData.position, null, out localPoint)) {
                float pct = Mathf.InverseLerp(progress.rectTransform.rect.xMin, progress.rectTransform.rect.xMax, localPoint.x);
                SkipToPercent(pct);
            }
        }

        private void SkipToPercent(float pct) {
            VideoPlayerCallBacks.onRewind?.Invoke(pct);
        }

    }
}
