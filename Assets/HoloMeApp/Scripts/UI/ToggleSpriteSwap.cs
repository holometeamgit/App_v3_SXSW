using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleSpriteSwap : MonoBehaviour {

    [SerializeField]
    Sprite imgToggleOn;

    [SerializeField]
    Sprite imgToggleOff;

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
        if (imgToggleOn == null || imgToggleOff == null) {
            HelperFunctions.DevLogError(nameof(ToggleSpriteSwap) + " was missing image assignment, please assign for the component to work");
        } else {
            toggleRef = GetComponent<Toggle>();
            initialOnValue = toggleRef.isOn;
            toggleRef.onValueChanged.AddListener(x => { ToggleSprite(x, toggleRef); });
            ToggleSprite(toggleRef.isOn, toggleRef);
        }
    }

    private void ToggleSprite(bool isOn, Toggle toggle) {
        toggle.image.sprite = isOn ? imgToggleOn : imgToggleOff;
    }
}
