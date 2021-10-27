
using UnityEngine;
using UnityEngine.UI;

namespace Beem.KeyBoard {
    /// <summary>
    /// KeyBoard Actions
    /// </summary>
    public class InputFieldAction : MonoBehaviour {

        [SerializeField]
        private InputField _inputField;

        public void OpenKeyBoard(bool isOpened) {
            KeyBoardConstructor.onInputShow?.Invoke(isOpened, _inputField);
        }
    }
}
