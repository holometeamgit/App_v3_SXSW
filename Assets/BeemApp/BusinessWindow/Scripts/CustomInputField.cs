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
    private InputField _inputField;

    [SerializeField]
    private MobileInputField _mobileInputField;

    [SerializeField]
    private int _keyboardHeight = 350;

    public static Action<bool, int> OnShowKeyboard;

    [SerializeField]
    private bool isLink;

    public string Text {
        get {
            return _mobileInputField.Text;
        }
        set {
            _mobileInputField.Text = value;
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
        _inputField.onEndEdit.AddListener(ChangeText);
        _mobileInputField.OnFocusChanged += OnMobileFocus;
    }

    private void OnDisable() {
        _inputField.onEndEdit.RemoveListener(ChangeText);
        _mobileInputField.OnFocusChanged -= OnMobileFocus;
    }

    private void Update() {
        _mobileInputField.SetRectNative();
    }

    private void OnMobileFocus(bool focus) {
        OnShowKeyboard?.Invoke(focus, focus ? _keyboardHeight : 0);
    }

    private void ChangeText(string text) {
        _clearBtn.SetActive(text.Length > 0);
    }

    /// <summary>
    /// Update Text
    /// </summary>
    /// <param name="text"></param>
    public void UpdateText(string text) {
        Text = text;
        ChangeText(text);
    }

    /// <summary>
    /// Clear Btn
    /// </summary>
    public void Clear() {
        UpdateText(string.Empty);
    }
}
