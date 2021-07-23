using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.Utility.UnityConsole {

    /// <summary>
    /// Search symbols in logs
    /// </summary>
    [RequireComponent(typeof(InputField))]
    public class KeyWordsField : MonoBehaviour {
        private InputField inputField;

        private void Awake() {
            inputField = GetComponent<InputField>();
        }

        private void OnEnable() {
            inputField.onEndEdit.AddListener(ChangeKeys);
        }

        private void OnDisable() {
            inputField.onEndEdit.RemoveListener(ChangeKeys);
        }

        private void ChangeKeys(string keys) {
            LogData.SetInputKeys(keys);
        }
    }
}
