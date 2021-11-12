using Mopsicus.Plugins;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
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
        private float _baseHeight = 165;
        [SerializeField]
        private float _baseShift = 50;

        [SerializeField]
        private float _speed = 10;

        private Coroutine coroutine;

        public override void RefreshData(InputField inputField) {
            if (inputField.textComponent.text.Length > 0) {
                ChangeInputHeight(_baseHeight + inputField.textComponent.preferredHeight - _baseShift);
            } else {
                ChangeInputHeight(_baseHeight);
            }
        }

        private void ChangeInputHeight(float height) {
            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, height);
        }

    }
}
