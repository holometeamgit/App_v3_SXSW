
using UnityEngine;
using UnityEngine.UI;

namespace Beem.KeyBoard {
    /// <summary>
    /// KeyBoard Actions
    /// </summary>
    public class UITextAction : MonoBehaviour {

        [SerializeField]
        private UITextField _uiTextField;

        [SerializeField]
        public InputField.OnChangeEvent onValueChanged;

        [SerializeField]
        public InputField.SubmitEvent onEndEdit;

        public void OpenKeyBoard(bool isOpened) {
            KeyBoardConstructor.onUITextShow?.Invoke(isOpened, onValueChanged, onEndEdit, _uiTextField);
        }
    }
}
