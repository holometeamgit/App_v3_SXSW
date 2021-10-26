using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.KeyBoard {
    /// <summary>
    /// KeyBoard Facade
    /// </summary>
    public class KeyBoardFacade : MonoBehaviour {
        [SerializeField]
        private RectTransform _rectTransform;
        [SerializeField]
        private InputField _inputField;

        [SerializeField]
        private List<AbstractKeyBoardSettings> _inputFieldSettings = new List<AbstractKeyBoardSettings>();

        private void OnEnable() {
            UpdateText();
            _inputField.onValueChanged.AddListener(UpdateText);
        }

        private void OnDisable() {
            _inputField.onValueChanged.RemoveListener(UpdateText);
        }

        private void UpdateText(string text = "") {
            foreach (AbstractKeyBoardSettings item in _inputFieldSettings) {
                item.RefreshData(_inputField);
            }
        }



    }
}
