using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace Beem.KeyBoard {
    /// <summary>
    /// Position View
    /// </summary>
    public class KeyBoardPositionView : MonoBehaviour {
        [SerializeField]
        private RectTransform _rectTransform;
        [SerializeField]
        private int keyBoardHeightiOS = 840;
        [SerializeField]
        private int keyBoardHeightAndroid = 800;

        public RectTransform RectTransform => _rectTransform;

        /// <summary>
        /// Update Keyboard Position
        /// </summary>
        public void UpdatePosition(bool isShown, int height = 0) {

            if (isShown) {
#if UNITY_IOS
                ChangePosition(Mathf.Max(keyBoardHeightiOS, height));
#elif UNITY_ANDROID
                ChangePosition(Mathf.Max(keyBoardHeightAndroid, height));
#endif
            } else {
                ChangePosition(height);
            }
        }

        private void ChangePosition(int height) {
            _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, height);
        }

    }
}
