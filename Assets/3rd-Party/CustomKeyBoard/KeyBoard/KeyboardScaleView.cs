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

        private const float EPSILON = 0.1f;

        public override void RefreshData(InputField inputField) {
            var x = new GUIStyle {
                font = inputField.textComponent.font,
                fontSize = inputField.textComponent.fontSize
            };
            var textTranform = inputField.textComponent.GetComponent<RectTransform>();
            var words = inputField.text.Split(' ');
            var currentWidth = 0f;
            var currentHeight = 0f;

            foreach (string word in words) {
                var size = x.CalcSize(new GUIContent(word));
                currentWidth += size.x;
                if (currentWidth >= textTranform.sizeDelta.x) {
                    currentHeight = _baseShift * (Mathf.FloorToInt(currentWidth / textTranform.sizeDelta.x));
                }
            }

            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _baseHeight + currentHeight);
            textTranform.sizeDelta = new Vector2(textTranform.sizeDelta.x, _baseShift + currentHeight);
        }

    }
}
