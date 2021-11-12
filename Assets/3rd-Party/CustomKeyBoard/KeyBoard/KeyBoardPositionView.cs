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
        private float keyBoardHeightiOS = 830;
        [SerializeField]
        private float keyBoardHeightAndroid = 680;

        [SerializeField]
        private float _speed = 10;

        private Coroutine coroutine;

        /// <summary>
        /// Update Keyboard Position
        /// </summary>
        public void UpdatePosition(bool isShown) {

#if UNITY_IOS
            UpdatePosition(isShown, keyBoardHeightiOS);
#elif UNITY_ANDROID
            UpdatePosition(isShown, keyBoardHeightAndroid);
#endif

        }

        /// <summary>
        /// Update keyboard position
        /// </summary>
        /// <param name="height"></param>
        public void UpdatePosition(bool isShown, float height) {
            if (isShown) {
                ChangePosition(height);
            }
        }

        private void ChangePosition(float height) {
            _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, height);

        }

    }
}
