
using UnityEngine;
using UnityEngine.UI;

namespace Beem.KeyBoard {
    /// <summary>
    /// KeyBoard Actions
    /// </summary>
    public class KeyBoardAction : MonoBehaviour {

        [SerializeField]
        private InputField.OnChangeEvent onChangeEvent;
        [SerializeField]
        private InputField.SubmitEvent submitEvent;

        public void OpenKeyBoard(bool isOpened) {
            KeyBoardConstructor.onShow?.Invoke(isOpened, onChangeEvent, submitEvent);
        }
    }
}
