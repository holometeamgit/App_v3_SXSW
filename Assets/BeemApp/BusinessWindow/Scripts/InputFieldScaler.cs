using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputFieldScaler : MonoBehaviour {
    [SerializeField]
    private RectTransform _rect;
    [SerializeField]
    private Mover _mover;

    private float _defaultHeight = 1520f;

    private void OnEnable() {
        _defaultHeight = _rect.rect.height;
        CustomInputField.OnShowKeyboard += OnInputField;
    }

    private void OnDisable() {
        CustomInputField.OnShowKeyboard -= OnInputField;
    }

    private void OnInputField(bool isShown, int height) {
        _rect.sizeDelta = new Vector2(_rect.sizeDelta.x, _defaultHeight + (isShown ? height : 0));
        _mover.UpdatePosition();
    }
}
