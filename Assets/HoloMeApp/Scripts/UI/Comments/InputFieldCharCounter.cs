using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldCharCounter : MonoBehaviour {
    [SerializeField]
    TMP_InputField _inputField;

    [SerializeField]
    TMP_Text _txtCount;

    public void UpdateCountCharacters(string text) {
        _txtCount.text = text.Length + "/" + _inputField.characterLimit;
    }
}
