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

        /// <summary>
        /// Update Keyboard Position
        /// </summary>
        public void UpdatePosition(bool isShown, int height = 0) {

            if (isShown) {
#if UNITY_IOS
                Debug.LogError($"OnShowKeyboard3 isShown= {isShown}, height = {Mathf.Max(keyBoardHeightiOS, height)}");
                ChangePosition(Mathf.Max(keyBoardHeightiOS, height));
#elif UNITY_ANDROID
                Debug.LogError($"OnShowKeyboard3 isShown= {isShown}, height = {Mathf.Max(keyBoardHeightAndroid, height)}");
                ChangePosition(Mathf.Max(keyBoardHeightAndroid, height));
#endif
            } else {
                Debug.LogError($"OnShowKeyboard3 isShown= {isShown}, height = {height}");
                ChangePosition(height);
            }

            Debug.LogError($"OnShowKeyboard4 isShown= {isShown}, height = {KeyboardHeight.GetKeyboardHeight(false)}");
        }

        private void ChangePosition(int height) {
            _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, height);
        }

    }
}
