using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// CTA Link Window
/// </summary>
public class CTALinkOptionsWindow : MonoBehaviour, IBlindView {

    [SerializeField]
    private CustomInputField[] _customInputFields;

    [SerializeField]
    private Button _updateDataBtn;

    [SerializeField]
    private GameObject _warning;

    [SerializeField]
    private TMP_Text _warningText;

    private string _warningTxt;

    private const string SUCCESS_OPTIONS_VIEW = "SuccessOptionsView";
    private const string CTA_LINK_OPTIONS_VIEW = "CTALinkOptionsView";

    /// <summary>
    /// Show Window
    /// </summary>
    public void Show(params object[] objects) {

        if (objects != null && objects.Length > 0) {
            foreach (var item in objects) {
                if (item is string) {
                    _warningTxt = item as string;
                }
            }
        }

        gameObject.SetActive(true);

        _warning.SetActive(objects != null && objects.Length > 0);

        if (!string.IsNullOrEmpty(_warningTxt)) {
            _warningText.text = _warningTxt;
        }

        CheckText();

        foreach (var item in _customInputFields) {
            item.GetInputField.onValueChanged.AddListener(OnValueChanged);
        }
    }

    /// <summary>
    /// Hide Window
    /// </summary>
    public void Hide() {
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
            var valid = item.IsValid();
            await valid;
            if (valid.IsFaulted || !valid.Result) {
                WarningConstructor.ActivateDoubleButton(message: "Something went wrong", buttonOneText: "Retry", buttonTwoText: "Cancel", onButtonOnePress: UpdateDataButton, isWarning: true);
                return;
            }
        }

        SuccessOptionsData data = new SuccessOptionsData(title: "Edit CTA", description: "The CTA information has\nbeen updated", backEvent: () => BlindOptionsConstructor.Show(CTA_LINK_OPTIONS_VIEW));

        BlindOptionsConstructor.Show(SUCCESS_OPTIONS_VIEW, data);
    }

}
