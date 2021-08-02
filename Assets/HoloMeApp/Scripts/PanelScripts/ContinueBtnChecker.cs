using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueBtnChecker : MonoBehaviour
{
    [SerializeField]
    [Tooltip("This inputFields must be filled for enable btn")]
    private List <InputFieldController> _filledInputFieldControllers;

    [SerializeField]
    [Tooltip("This toggles must be is On for enable btn")]
    private List<Toggle> _isOnToggles;

    [SerializeField]
    [Tooltip("This btn will be interactable if all requirements are met ")]
    private Button _button;

    private void CheckRequirements(bool value) {
        CheckRequirements();
    }

    private void CheckRequirements() {
        _button.interactable = IsInputFieldsFilled() && IsOnToggles();
    }

    private bool IsOnToggles() {
        foreach (var toggle in _isOnToggles) {
            if (!toggle.isOn)
                return false;
        }

        return true;
    }

    private bool IsInputFieldsFilled() {
        foreach (var inputField in _filledInputFieldControllers) {
            if (inputField.text.Length < 1)
                return false;
        }

        return true;
    }

    private void SubscribesOnEndInputFields() {
        foreach(var inputField in _filledInputFieldControllers) {
            inputField.OnEndEditPassword.AddListener(CheckRequirements);
        }
    }

    private void DescribesOnEndInputFields() {
        foreach (var inputField in _filledInputFieldControllers) {
            inputField.OnEndEditPassword.RemoveListener(CheckRequirements);
        }
    }

    private void SubscribesOnChangeToggles() {
        foreach (var toggle in _isOnToggles) {
            toggle.onValueChanged.AddListener(CheckRequirements);
        }
    }

    private void DescribesOnChangeToggles() {
        foreach (var toggle in _isOnToggles) {
            toggle.onValueChanged.RemoveListener(CheckRequirements);
        }
    }

    private void OnEnable() {
        SubscribesOnEndInputFields();
        SubscribesOnChangeToggles();
        CheckRequirements();
    }

    private void OnDisable() {
        DescribesOnEndInputFields();
        DescribesOnChangeToggles();
        CheckRequirements();
    }

}
