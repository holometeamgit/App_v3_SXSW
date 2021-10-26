using UnityEngine;
namespace Beem.KeyBoard {
    /// <summary>
    /// Position View
    /// </summary>
    public class KeyBoardPositionView : MonoBehaviour {
        [SerializeField]
        private RectTransform _rectTransform;
        [SerializeField]
        private int androidShift = 826;

        public void UpdatePosition(int basePosition) {
            int shift = 0;
            //#if UNITY_ANDROID
            shift = androidShift;
            //#endif
            int _basePosition = shift;
            Vector2 position = _rectTransform.anchoredPosition;
            position.y = _basePosition;
            _rectTransform.anchoredPosition = position;
        }
    }
}
