using UnityEngine;
using UnityEngine.UI;

namespace Beem.KeyBoard {
    /// <summary>
    /// Char Limit View
    /// </summary>
    public class KeyBoardCharLimitView : AbstractKeyBoardSettings {

        [SerializeField]
        private Text _textLimit;

        public override void RefreshData(InputField inputField) {
            if (inputField.characterLimit > 0) {
                _textLimit.text = inputField.text.Length + "/" + inputField.characterLimit;
            } else {
                _textLimit.text = string.Empty;
            }
        }
    }
}
