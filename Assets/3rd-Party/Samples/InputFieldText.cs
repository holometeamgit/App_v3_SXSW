using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldText : MonoBehaviour {
    [SerializeField]
    private GameObject _placeHolder;

    [SerializeField]
    private Text _inputFieldText;

    public void InputText(string text) {
        if (string.IsNullOrEmpty(text)) {
            _placeHolder.SetActive(true);
            _inputFieldText.text = string.Empty;
        } else {
            _placeHolder.SetActive(false);
            _inputFieldText.text = text;
        }
    }
}
