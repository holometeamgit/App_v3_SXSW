
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.KeyBoard {
    /// <summary>
    /// KeyBoard Actions
    /// </summary>
    public class InputFieldAction : MonoBehaviour {

        [SerializeField]
        private TMP_InputField _inputField;

        public void OpenKeyBoard(bool isOpened) {
            // KeyBoardConstructor.onShow?.Invoke(isOpened, onValueChanged, onEndEdit);
        }
    }
}
