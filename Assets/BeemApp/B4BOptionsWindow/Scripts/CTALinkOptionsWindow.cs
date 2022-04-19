using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// CTA Link Editor
/// </summary>
public class CTALinkOptionsWindow : MonoBehaviour {

    [SerializeField]
    private CustomInputField[] _customInputFields;

    [SerializeField]
    private Button _updateDataBtn;

    [SerializeField]
    private SwipePopUp _swipePopUp;

    /// <summary>
    /// Show Window
    /// </summary>
    public void Show() {
        gameObject.SetActive(true);
        CheckText();

        foreach (var item in _customInputFields) {
            item.GetInputField.onValueChanged.AddListener(OnValueChanged);
        }

        _swipePopUp.Show();
        _swipePopUp.onHid += OnClose;
    }

    /// <summary>
    /// Hide Window
    /// </summary>
    public void Hide() {
        _swipePopUp.Hide();
    }

    private void OnClose() {
        _swipePopUp.onHid -= OnClose;
        foreach (var item in _customInputFields) {
            item.GetInputField.onValueChanged.RemoveListener(OnValueChanged);
        }
        gameObject.SetActive(false);
    }

    private void CheckText() {
        foreach (var item in _customInputFields) {
            if (item.GetMobileInputField.Text.Length == 0) {
                _updateDataBtn.interactable = false;
                return;
            }
        }
        _updateDataBtn.interactable = true;
    }

    private void OnValueChanged(string text) {
        CheckText();
    }


    public void UpdateDataButton() {

    }

}
