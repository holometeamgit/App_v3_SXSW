using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Use this component to change text colour based on toggle status
/// </summary>
[RequireComponent(typeof(Toggle))]
public class ToggleTextColourSwap : MonoBehaviour {

    [Tooltip("Assign this if you need text to change colour as well")]
    [SerializeField]
    TextMeshProUGUI referenceText;
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
            HelperFunctions.DevLogError(nameof(ToggleTextColourSwap) + " was missing a toggle component, please add for the component to work");
        }
        if (referenceText == null) {
            HelperFunctions.DevLogError(nameof(ToggleTextColourSwap) + " was missing a text reference assignment, please assign for the component to work");
        }
        initialOnValue = toggleRef.isOn;
        originalColorRef = referenceText.color;
        toggleRef.onValueChanged.AddListener(x => { ToggleColour(x); });
        ToggleColour(toggleRef.isOn);
    }

    private void ToggleColour(bool isOn) {
        referenceText.color = isOn ? originalColorRef : colourToggledOff; ;
    }
}
