using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    [SerializeField]
    private GameObject _inputFields;

    [SerializeField]
    private GameObject _successEdit;

    private const int DELAY_FOR_SUCCESS = 3000;

    /// <summary>
    /// Show Window
    /// </summary>
    public void Show() {
        gameObject.SetActive(true);
        _inputFields.SetActive(true);
        _successEdit.SetActive(false);
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

    /// <summary>
    /// Update Data Button
    /// </summary>
    public async void UpdateDataButton() {
        foreach (var item in _customInputFields) {
            if (!item.IsValid()) {
                WarningConstructor.ActivateDoubleButton(message: "Something went wrong", buttonOneText: "Retry", buttonTwoText: "Cancel", onButtonOnePress: UpdateDataButton, isWarning: true);
                return;
            }
        }

        _inputFields.SetActive(false);
        _successEdit.SetActive(true);
        await Task.Delay(DELAY_FOR_SUCCESS);
        BusinessOptionsConstructor.OnShowLast?.Invoke();
        CTALinkOptionsConstructor.OnHide?.Invoke();
    }

}
