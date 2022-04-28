using Mopsicus.Plugins;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputFieldBtn : MonoBehaviour, IPointerClickHandler {

    [SerializeField]
    private InputField _inputField;
    [SerializeField]
    private MobileInputField _mobileInputField;

    public static Action<bool, int> OnShowKeyboard;

    private void OnEnable() {
        _mobileInputField.OnFocusChanged += OnMobileFocus;
        _mobileInputField.OnReturnPressed += OnReturnPressed;
        _inputField.onEndEdit.AddListener(OnEndEdit);
    }

    private void OnDisable() {
        _mobileInputField.OnFocusChanged -= OnMobileFocus;
        _mobileInputField.OnReturnPressed -= OnReturnPressed;
        _inputField.onEndEdit.RemoveListener(OnEndEdit);
    }

    private void Update() {
        _mobileInputField.SetRectNative();
    }

    private void OnReturnPressed() {
        Debug.LogError("OnReturnPressed");
    }

    private void OnMobileFocus(bool focus) {
        Debug.LogError("OnMobileFocus " + focus);
        if (focus) {
            OnShowKeyboard?.Invoke(true, 350);
        } else {
            OnShowKeyboard?.Invoke(false, 0);
        }
    }

    private void ActivateKeyboard() {
        Debug.LogError("ActivateKeyboard");
        //_mobileInputField.SetFocus(true);
        //OnShowKeyboard?.Invoke(true, 350);
    }

    private void DeactivateKeyboard() {
        Debug.LogError("DeactivateKeyboard");
        //_mobileInputField.SetFocus(false);
        //OnShowKeyboard?.Invoke(false, 0);
    }

    private void OnEndEdit(string text) {
        DeactivateKeyboard();
    }

    public void OnPointerClick(PointerEventData eventData) {
        ActivateKeyboard();
    }
}
