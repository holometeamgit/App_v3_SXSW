using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleColourSwap : MonoBehaviour {
    [SerializeField]
    Image referenceImage;
    [SerializeField]
    Color colourToggledOff;
    Color originalColorRef;

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
        if (referenceImage == null) {
            HelperFunctions.DevLogError(nameof(ToggleColourSwap) + " was missing image assignment, please assign for the component to work");
        } else {
            toggleRef = GetComponent<Toggle>();
            initialOnValue = toggleRef.isOn;
            originalColorRef = toggleRef.image.color;
            toggleRef.onValueChanged.AddListener(x => { ToggleColour(x, toggleRef); });
            ToggleColour(toggleRef.isOn, toggleRef);
        }
    }

    private void ToggleColour(bool isOn, Toggle toggle) {
        toggle.image.color = isOn ? originalColorRef : colourToggledOff;
    }
}
