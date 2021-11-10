using UnityEngine;
namespace Beem.KeyBoard {
    /// <summary>
    /// Position View
    /// </summary>
    public class KeyBoardPositionView : MonoBehaviour {
        [SerializeField]
        private RectTransform _rectTransform;
        [SerializeField]
        private float keyBoardHeightiOS = 830;
        [SerializeField]
        private float keyBoardHeightAndroid = 680;

        /// <summary>
        /// Update Keyboard Position
        /// </summary>
        public void UpdatePosition() {
#if UNITY_IOS
            ChangePosition(keyBoardHeightiOS);
#elif UNITY_ANDROID
            ChangePosition(keyBoardHeightAndroid);
#endif

        }

        /// <summary>
        /// Update keyboard position
        /// </summary>
        /// <param name="height"></param>
        public void UpdatePosition(int height) {
            ChangePosition(height);
        }

        private void ChangePosition(float height) {
            float _basePosition = height;
            Vector2 position = _rectTransform.anchoredPosition;
            position.y = _basePosition;
            _rectTransform.anchoredPosition = position;
        }
    }
}
