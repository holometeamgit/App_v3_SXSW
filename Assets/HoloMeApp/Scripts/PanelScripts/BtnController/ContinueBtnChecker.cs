using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueBtnChecker : BtnInteractionRequirementChecker {
    [SerializeField]
    [Tooltip("This inputFields must be filled for enable btn")]
    private List<InputFieldController> _filledInputFieldControllers;

    [SerializeField]
    [Tooltip("This toggles must be is On for enable btn")]
    private List<Toggle> _isOnToggles;

    protected override bool CheckRequirements() {
        return _canInteract;
    }

    protected override void OnEnable() {
        base.OnEnable();
        SubscribesOnEndInputFields();
        SubscribesOnChangeToggles();
        CheckContinueBtnRequirements();
        _onRequirementsUpdated?.Invoke();
    }

    protected override void OnDisable() {
        base.OnDisable();
        DescribesOnEndInputFields();
        DescribesOnChangeToggles();
        CheckContinueBtnRequirements();
    }

    private void CheckContinueBtnRequirements(bool value) {
        CheckContinueBtnRequirements();
    }

    private void CheckContinueBtnRequirements() {
        bool prevCanInteract = _canInteract;
        _canInteract = IsInputFieldsFilled() && IsOnToggles();

        if (_canInteract != prevCanInteract)
            _onRequirementsUpdated?.Invoke();
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
            if (string.IsNullOrWhiteSpace(inputField.text))
                return false;
        }

        return true;
    }

    private void SubscribesOnEndInputFields() {
        foreach (var inputField in _filledInputFieldControllers) {
            inputField.onValueChanged.AddListener(CheckContinueBtnRequirements);
            inputField.OnEndEditPassword.AddListener(CheckContinueBtnRequirements);
        }
    }

    private void DescribesOnEndInputFields() {
        foreach (var inputField in _filledInputFieldControllers) {
            inputField.onValueChanged.RemoveListener(CheckContinueBtnRequirements);
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
}
