
using UnityEngine;
using UnityEngine.UI;

namespace Beem.KeyBoard {
    /// <summary>
    /// KeyBoard Actions
    /// </summary>
    public class UITextAction : MonoBehaviour {

        [SerializeField]
        private UITextField _uiTextField;

        public void OpenKeyBoard(bool isOpened) {
            KeyBoardConstructor.onUITextShow?.Invoke(isOpened, _uiTextField);
        }
    }
}
