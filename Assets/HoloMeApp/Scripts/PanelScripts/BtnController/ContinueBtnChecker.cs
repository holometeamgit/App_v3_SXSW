using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueBtnChecker : BtnInteractionRequirementChecker {
    [SerializeField]
    [Tooltip("This inputFields must be filled for enable btn")]
    private List <InputFieldController> _filledInputFieldControllers;

    [SerializeField]
    [Tooltip("This toggles must be is On for enable btn")]
    private List<Toggle> _isOnToggles;

    private void CheckContinueBtnRequirements(bool value) {
        CheckRequirements();
    }

    public void CheckContinueBtnRequirements() {
        _canInteract = IsInputFieldsFilled() && IsOnToggles();
        HelperFunctions.DevLog("CheckRequirements " + (IsInputFieldsFilled() && IsOnToggles()));
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
            inputField.OnEndEditPassword.AddListener(CheckContinueBtnRequirements);
        }
    }

    private void DescribesOnEndInputFields() {
        foreach (var inputField in _filledInputFieldControllers) {
            inputField.OnEndEditPassword.RemoveListener(CheckContinueBtnRequirements);
        }
    }

    private void SubscribesOnChangeToggles() {
        foreach (var toggle in _isOnToggles) {
            toggle.onValueChanged.AddListener(CheckContinueBtnRequirements);
        }
    }

    private void DescribesOnChangeToggles() {
        foreach (var toggle in _isOnToggles) {
            toggle.onValueChanged.RemoveListener(CheckContinueBtnRequirements);
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
