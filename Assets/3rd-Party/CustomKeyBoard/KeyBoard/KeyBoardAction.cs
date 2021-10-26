
using UnityEngine;

namespace Beem.KeyBoard {
    /// <summary>
    /// KeyBoard Actions
    /// </summary>
    public class KeyBoardAction : MonoBehaviour {
        public void OpenKeyBoard(bool isOpened) {
            KeyBoardConstructor.onShow?.Invoke(isOpened);
        }
    }
}
