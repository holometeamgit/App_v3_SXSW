using Mopsicus.Plugins;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Custom Input Field
/// </summary>
public class CustomInputField : MonoBehaviour {
    [SerializeField]
    private GameObject _clearBtn;

    [SerializeField]
    private MobileInputField _mobileInputField;

    [SerializeField]
    private InputField _inputField;

    public MobileInputField GetMobileInputField {
        get {
            return _mobileInputField;
        }
    }

    public InputField GetInputField {
        get {
            return _inputField;
        }
    }

    private void OnEnable() {
        _inputField.onEndEdit.AddListener(ChangeText);
        _mobileInputField.OnReturnPressedEvent.AddListener(OnReturnedPressed);
    }

    private void OnDisable() {
        _inputField.onEndEdit.RemoveListener(ChangeText);
        _mobileInputField.OnReturnPressedEvent.RemoveListener(OnReturnedPressed);
    }

    private void OnReturnedPressed() {
        ChangeText(_mobileInputField.Text);
    }

    private void ChangeText(string text) {
        _clearBtn.SetActive(text.Length > 0);
    }

    /// <summary>
    /// Update Text
    /// </summary>
    /// <param name="text"></param>
    public void UpdateText(string text) {
        _mobileInputField.Text = text;
    }

    /// <summary>
    /// Clear Btn
    /// </summary>
    public void Clear() {
        _mobileInputField.Text = string.Empty;
    }
}
