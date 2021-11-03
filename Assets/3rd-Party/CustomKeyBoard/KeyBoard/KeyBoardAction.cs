
using UnityEngine;
using UnityEngine.UI;

namespace Beem.KeyBoard {
    /// <summary>
    /// KeyBoard Actions
    /// </summary>
    public class KeyBoardAction : MonoBehaviour {

        [SerializeField]
        private InputField.OnChangeEvent onValueChanged;
        [SerializeField]
        private InputField.SubmitEvent onEndEdit;

        public void OpenKeyBoard(bool isOpened) {
            KeyBoardConstructor.onShow?.Invoke(isOpened, onValueChanged, onEndEdit);
        }
    }
}
