using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Use this component to change and image's colour property based on a toggle component status
/// </summary>
[RequireComponent(typeof(Toggle))]
public class ToggleColourSwap : MonoBehaviour {

    [SerializeField]
    Color colourToggledOff;
    Color originalColorRef;

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
        toggleRef = GetComponent<Toggle>();
        if (toggleRef == null) {
            HelperFunctions.DevLogError(nameof(ToggleColourSwap) + " was missing image assignment, please assign for the component to work");
        }
        initialOnValue = toggleRef.isOn;
        originalColorRef = toggleRef.image.color;
        toggleRef.onValueChanged.AddListener(x => { ToggleColour(x, toggleRef); });
        ToggleColour(toggleRef.isOn, toggleRef);

    }

    private void ToggleColour(bool isOn, Toggle toggle) {
        toggle.image.color = isOn ? originalColorRef : colourToggledOff;
    }
}
