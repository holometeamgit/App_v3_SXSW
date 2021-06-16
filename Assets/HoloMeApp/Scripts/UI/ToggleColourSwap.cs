using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleColourSwap : MonoBehaviour {
    [SerializeField]
    Image referenceImage;
    [SerializeField]
    Color colourToggledOff;
    Color originalColorRef;


    private void Awake() {
        if (referenceImage == null) {
            HelperFunctions.DevLogError(nameof(ToggleColourSwap) + " was missing image assignment, please assign for the component to work");
        } else {
            var toggle = GetComponent<Toggle>();
            originalColorRef = toggle.image.color;
            toggle.onValueChanged.AddListener(x => { ToggleColour(x, toggle); });
            ToggleColour(toggle.isOn, toggle);
        }
    }

    private void ToggleColour(bool isOn, Toggle toggle) {
        toggle.image.color = isOn ? originalColorRef : colourToggledOff;
    }
}
