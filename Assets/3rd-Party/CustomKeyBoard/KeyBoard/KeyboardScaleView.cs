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
            Vector2 size = _rectTransform.sizeDelta;

            Debug.Log(GetLineCount(inputField.textComponent.cachedTextGenerator));

            size.y = _baseHeight + _baseShift * GetLineCount(inputField.textComponent.cachedTextGenerator);

            _rectTransform.sizeDelta = size;
        }

        private int GetLineCount(TextGenerator textGenerator) {

            int lineCount = 0;

            if (textGenerator.characterCount > 1) {
                for (int i = 1; i < textGenerator.characterCount; i++) {
                    Debug.Log(textGenerator.characters[i - 1].cursorPos.y + "," + textGenerator.characters[i].cursorPos.y);
                    if (Mathf.Abs(textGenerator.characters[i - 1].cursorPos.y - textGenerator.characters[i].cursorPos.y) > EPSILON) {
                        lineCount++;
                    }
                }
            }

            return lineCount;
        }


    }
}
