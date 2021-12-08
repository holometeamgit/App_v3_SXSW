using UnityEngine;
using UnityEngine.UI;

namespace Beem.KeyBoard {
    /// <summary>
    /// Color View
    /// </summary>
    public class KeyboardColorView : AbstractKeyBoardSettings {
        [SerializeField]
        private Image _inputImg;
        [SerializeField]
        private Color _enableInputColor;
        [SerializeField]
        private Color _disableInputColor;

        public override void RefreshData(InputField inputField) {
            _inputImg.color = inputField.text.Length > 0 ? _enableInputColor : _disableInputColor;
        }
    }
}
