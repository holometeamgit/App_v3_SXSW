using Mopsicus.Plugins;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
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

    public string Text {
        get {
            return _inputField.text;
        }
    }

    public InputField GetInputField {
        get {
            return _inputField;
        }
    }

    public async Task<bool> IsValid() {
        if (isLink) {
            UnityWebRequest webRequest = UnityWebRequest.Get(Text);
            await webRequest.SendWebRequest();
            return webRequest.result == UnityWebRequest.Result.Success;
        } else {
            return true;
        }
    }

    private void OnEnable() {
        _inputField.onValueChanged.AddListener(ChangeText);
        //_mobileInputField.OnReturnPressedEvent.AddListener(OnReturnedPressed);
    }

    private void OnDisable() {
        _inputField.onValueChanged.RemoveListener(ChangeText);
        //_mobileInputField.OnReturnPressedEvent.RemoveListener(OnReturnedPressed);
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
