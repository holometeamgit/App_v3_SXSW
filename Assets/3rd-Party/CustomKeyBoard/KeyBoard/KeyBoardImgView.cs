using UnityEngine;
using UnityEngine.UI;

namespace Beem.KeyBoard {
    /// <summary>
    /// Image View
    /// </summary>
    public class KeyBoardImgView : AbstractKeyBoardSettings {
        [SerializeField]
        private Image _returnImg;
        [SerializeField]
        private Sprite _enableReturnImg;
        [SerializeField]
        private Sprite _disableReturnImg;

        public override void RefreshData(InputField inputField) {
            _returnImg.sprite = inputField.text.Length > 0 ? _enableReturnImg : _disableReturnImg;
        }
    }
}
