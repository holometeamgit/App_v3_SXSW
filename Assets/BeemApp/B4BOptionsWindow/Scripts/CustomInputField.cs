using Mopsicus.Plugins;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

    [SerializeField]
    private bool isLink;

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

    public bool IsValid() {
        if (isLink) {
            string s = _mobileInputField.Text;
            Uri resultURI;
            if (!Regex.IsMatch(s, @"^http(s)?:\/\/", RegexOptions.IgnoreCase))
                s = "https://" + s;

            if (Uri.TryCreate(s, UriKind.Absolute, out resultURI))
                return (resultURI.Scheme == Uri.UriSchemeHttp ||
                        resultURI.Scheme == Uri.UriSchemeHttps);

            return false;
        }

        return true;
    }

    private void OnEnable() {
        _inputField.onValueChanged.AddListener(ChangeText);
        _mobileInputField.OnReturnPressedEvent.AddListener(OnReturnedPressed);
    }

    private void OnDisable() {
        _inputField.onValueChanged.RemoveListener(ChangeText);
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
        ChangeText(text);
    }

    /// <summary>
    /// Clear Btn
    /// </summary>
    public void Clear() {
        UpdateText(string.Empty);
    }
}
