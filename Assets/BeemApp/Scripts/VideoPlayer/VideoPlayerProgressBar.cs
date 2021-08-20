using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Beem.Video {

    /// <summary>
    /// Progress Bar
    /// </summary>
    public class VideoPlayerProgressBar : MonoBehaviour, IPointerDownHandler, IDragHandler {
        [Header("Progress")]
        [SerializeField]
        private Image progress;

        [SerializeField]
        private UnityEvent<float> onRewind;

        private void SkipToPercent(float pct) {
            onRewind?.Invoke(pct);
        }

        public void OnPointerDown(PointerEventData eventData) {
            TrySkip(eventData);
        }

        public void OnDrag(PointerEventData eventData) {
            TrySkip(eventData);
        }

        private void TrySkip(PointerEventData eventData) {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(progress.rectTransform, eventData.position, null, out localPoint)) {
                float pct = Mathf.InverseLerp(progress.rectTransform.rect.xMin, progress.rectTransform.rect.xMax, localPoint.x);
                SkipToPercent(pct);
            }
        }
    }
}
