using UnityEngine;
namespace Beem.KeyBoard {
    /// <summary>
    /// Position View
    /// </summary>
    public class KeyBoardPositionView : MonoBehaviour {
        [SerializeField]
        private RectTransform _rectTransform;
        [SerializeField]
        private float screenPercent = 826;

        public void UpdatePosition() {
            float _basePosition = screenPercent;
            Vector2 position = _rectTransform.anchoredPosition;
            position.y = _basePosition;
            _rectTransform.anchoredPosition = position;
        }
    }
}
