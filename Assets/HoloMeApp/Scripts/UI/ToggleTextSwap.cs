﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Use this component to change text based on a toggle component status
/// </summary>
[RequireComponent(typeof(Toggle))]
public class ToggleTextSwap : MonoBehaviour {
    [SerializeField]
    TextMeshProUGUI textComponentReference;

    [SerializeField]
    string txtToggledOn;

    [SerializeField]
    string txtToggledOff;

    [Tooltip("Reset to the default status when gameobject is enabled")]
    [SerializeField]
    bool resetOnEnable;
    bool initialOnValue;
    Toggle toggleRef;

    private void OnEnable() {
        if (resetOnEnable) {
            toggleRef.isOn = initialOnValue;
        }
    }

    private void Awake() {
        if (textComponentReference == null) {
            HelperFunctions.DevLogError(nameof(ToggleTextSwap) + " was missing text component assignment, please assign for the component to work");
        } else {
            toggleRef = GetComponent<Toggle>();
            initialOnValue = toggleRef.isOn;
            toggleRef.onValueChanged.AddListener(x => { ToggleText(x); });
            ToggleText(toggleRef.isOn);
        }
    }

    private void ToggleText(bool isOn) {
        textComponentReference.text = isOn ? txtToggledOn : txtToggledOff;
    }
}
