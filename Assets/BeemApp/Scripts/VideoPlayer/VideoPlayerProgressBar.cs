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
    /// Progress bar
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class VideoPlayerProgressBar : MonoBehaviour, IDragHandler, IPointerDownHandler {

        private Image progress;

        private CancellationTokenSource cancelTokenSource;

        private void Awake() {
            progress = GetComponent<Image>();
        }

        public async void UpdateProgressBar(VideoPlayer videoPlayer) {
            progress.fillAmount = 0f;
            cancelTokenSource = new CancellationTokenSource();
            try {
                while (true) {
                    if (videoPlayer.frameCount > 0) {
                        progress.fillAmount = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
                    }
                    await Task.Yield();
                }
            } finally {
                if (cancelTokenSource != null) {
                    cancelTokenSource.Dispose();
                    cancelTokenSource = null;
                }
            }
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

        private void OnDestroy() {
            Clear();
        }

        /// <summary>
        /// Clear Info
        /// </summary>
        public void Clear() {
            if (cancelTokenSource != null) {
                cancelTokenSource.Cancel();
                cancelTokenSource = null;
            }
        }
    }
}
