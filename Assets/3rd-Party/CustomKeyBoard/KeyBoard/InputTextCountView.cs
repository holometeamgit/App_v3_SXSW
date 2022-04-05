
using UnityEngine;
using UnityEngine.UI;

namespace Beem.KeyBoard {
    /// <summary>
    /// Text Counter
    /// </summary>
    public class InputTextCountView : MonoBehaviour {

        [SerializeField]
        private InputField _inputField;
        [SerializeField]
        private Text _text;

        private void OnEnable() {
            _inputField.onValueChanged.AddListener(UpdateText);
        }

        private void OnDisable() {
            _inputField.onValueChanged.RemoveListener(UpdateText);
        }

        private void UpdateText(string text) {
            _text.text = text.Length + "/" + _inputField.characterLimit;
        }
    }
}
