
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Beem.KeyBoard {
    /// <summary>
    /// KeyBoard Actions
    /// </summary>
    public class UITextAction : MonoBehaviour, IPointerClickHandler {

        [SerializeField]
        private InputField _inputField;

        public void OpenKeyBoard(bool isOpened) {
            if (isOpened) {
                KeyBoardConstructor.onShow?.Invoke(_inputField);
            } else {
                KeyBoardConstructor.onHide?.Invoke();
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
            OpenKeyBoard(true);
        }
    }
}
