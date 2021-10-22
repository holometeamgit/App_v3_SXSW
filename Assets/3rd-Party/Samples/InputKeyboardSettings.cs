using Mopsicus.Plugins;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputKeyboardSettings : MonoBehaviour {
    [SerializeField]
    private RectTransform _rectTransform;
    [SerializeField]
    private InputField _inputField;
    [SerializeField]
    private Text _textLimit;

    [Space]
    [SerializeField]
    private Image _returnImg;
    [SerializeField]
    private Sprite _enableReturnImg;
    [SerializeField]
    private Sprite _disableReturnImg;
    [Space]
    [SerializeField]
    private Image _inputImg;
    [SerializeField]
    private Color _enableInputColor;
    [SerializeField]
    private Color _disableInputColor;

    [Space]
    [SerializeField]
    private float _baseShift = 50;
    [SerializeField]
    private int _maxCharInLine = 26;

    private float _baseHeight = 165;
    private int _basePosition;

    private void OnEnable() {
        _baseHeight = _rectTransform.sizeDelta.y;
        UpdateTextLimit();
        _inputField.onValueChanged.AddListener(UpdateTextLimit);
    }

    private void OnDisable() {
        _inputField.onValueChanged.RemoveListener(UpdateTextLimit);
    }

    private void UpdateTextLimit(string text = "") {
        Vector2 size = _rectTransform.sizeDelta;
        size.y = _baseHeight + _baseShift * GetLineCount(text, _maxCharInLine);
        _rectTransform.sizeDelta = size;
        _textLimit.text = _inputField.text.Length + "/" + _inputField.characterLimit;
        _returnImg.sprite = _inputField.text.Length > 0 ? _enableReturnImg : _disableReturnImg;
        _inputImg.color = _inputField.text.Length > 0 ? _enableInputColor : _disableInputColor;
    }

    private int GetLineCount(string text, int maxCharInLine) {
        return Mathf.FloorToInt((float)text.Length / (float)maxCharInLine);
    }


    private void Update() {
        if (_basePosition == basePosition) {
            return;
        }
        UpdatePosition();
    }

    public void UpdatePosition() {
        _basePosition = basePosition;
        Vector2 position = _rectTransform.anchoredPosition;
        position.y = _basePosition;
        _rectTransform.anchoredPosition = position;
        Debug.Log(_rectTransform.anchoredPosition);
    }

    private int basePosition {
        get {
            return UniSoftwareKeyboardArea.SoftwareKeyboardArea.GetHeight(!_inputField.shouldHideMobileInput);
        }
    }
}
