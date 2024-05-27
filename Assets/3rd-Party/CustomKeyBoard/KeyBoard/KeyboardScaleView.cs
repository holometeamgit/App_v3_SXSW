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
        private float _baseHeight = 200;
        [Space]
        [SerializeField]
        private RectTransform _textRectTransform;
        [SerializeField]
        private RectTransform _placeHolderRectTransform;
        [SerializeField]
        private float _baseTextHeight = 140;
        [Space]
        [SerializeField]
        private float _baseShift = 50;

        public override void RefreshData(InputField inputField) {
            if (inputField.textComponent.text.Length > 0) {
                ChangeHeight(_rectTransform, _baseHeight + inputField.textComponent.preferredHeight - _baseShift);
                ChangeHeight(_textRectTransform, _baseTextHeight + inputField.textComponent.preferredHeight - _baseShift);
                ChangeHeight(_placeHolderRectTransform, _baseTextHeight + inputField.textComponent.preferredHeight - _baseShift);
            } else {
                ChangeHeight(_rectTransform, _baseHeight);
                ChangeHeight(_textRectTransform, _baseTextHeight);
                ChangeHeight(_placeHolderRectTransform, _baseTextHeight);
            }
        }

        private void ChangeHeight(RectTransform rectTransform, float height) {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
        }
    }
}
