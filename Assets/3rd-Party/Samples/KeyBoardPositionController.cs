using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(TMP_InputField))]
public class KeyBoardPositionController : MonoBehaviour {
    private RectTransform _rectTransform;
    private TMP_InputField _tmpInputField;

    private int _height;
    private const int INPUT_TEXT_HEIGHT = 120;

    private void OnEnable() {
        _rectTransform = GetComponent<RectTransform>();
        _tmpInputField = GetComponent<TMP_InputField>();
    }

    private void Update() {
        if (_height == height) {
            return;
        }
        UpdatePosition();
    }

    public void UpdatePosition() {
        _height = height;
        Vector2 vector2 = _rectTransform.anchoredPosition;
        vector2.y = _height;
        _rectTransform.anchoredPosition = vector2;
        Debug.Log(_rectTransform.anchoredPosition);
    }

    private int height {
        get {
            return UniSoftwareKeyboardArea.SoftwareKeyboardArea.GetHeight(false);// - INPUT_TEXT_HEIGHT * (_tmpInputField.shouldHideMobileInput ? 1 : 0);
        }
    }
}
