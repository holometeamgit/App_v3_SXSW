using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// CTA Link Window
/// </summary>
public class CTALinkOptionsWindow : MonoBehaviour {

    [SerializeField]
    private CustomInputField[] _customInputFields;

    [SerializeField]
    private Button _updateDataBtn;

    [SerializeField]
    private Mover _mover;

    [SerializeField]
    private GameObject _warning;

    /// <summary>
    /// Show Window
    /// </summary>
    public void Show(bool isWarning = false) {
        gameObject.SetActive(true);
        _warning.SetActive(isWarning);
        CheckText();

        foreach (var item in _customInputFields) {
            item.GetInputField.onValueChanged.AddListener(OnValueChanged);
        }

        _mover.ChangeState(true);
        _mover.onEndMoving += OnClose;
    }

    /// <summary>
    /// Hide Window
    /// </summary>
    public void Hide() {
        _mover.ChangeState(false);
    }

    private void OnClose(bool status) {
        if (!status) {
            _mover.onEndMoving -= OnClose;
            foreach (var item in _customInputFields) {
                item.GetInputField.onValueChanged.RemoveListener(OnValueChanged);
            }
            gameObject.SetActive(false);
        }
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
            var valid = item.IsValid();
            await valid;
            if (valid.IsFaulted || !valid.Result) {
                WarningConstructor.ActivateDoubleButton(message: "Something went wrong", buttonOneText: "Retry", buttonTwoText: "Cancel", onButtonOnePress: UpdateDataButton, isWarning: true);
                return;
            }
        }

        CTALinkOptionsConstructor.OnHide?.Invoke();
        SuccessOptionsConstructor.OnShow?.Invoke();
    }

}
