using UnityEngine;
using UnityEngine.UI;

namespace Beem.KeyBoard {
    /// <summary>
    /// Scale View
    /// </summary>
    public class KeyboardScaleView : AbstractKeyBoardSettings {

        [SerializeField]
        private RectTransform _rectTransform;
        [SerializeField]
        private float _baseShift = 50;
        [SerializeField]
        private float _baseHeight = 165;

        public override void RefreshData(InputField inputField) {
            Vector2 size = _rectTransform.sizeDelta;

            size.y = _baseHeight + _baseShift * GetLineCount(inputField.textComponent);
            _rectTransform.sizeDelta = size;
        }

        private int GetLineCount(Text text) {
            Canvas.ForceUpdateCanvases();
            return text.cachedTextGenerator.lines.Count;
        }

    }
}
