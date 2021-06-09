using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleSpriteSwap : MonoBehaviour {

    [SerializeField]
    Sprite imgToggleOn;

    [SerializeField]
    Sprite imgToggleOff;

    private void Awake() {
        if (imgToggleOn == null || imgToggleOff == null) {
            HelperFunctions.DevLogError(nameof(ToggleSpriteSwap) + " was missing image assignment, please assign for the component to work");
        } else {
            var toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(x => { ToggleSprite(x, toggle); });
            ToggleSprite(toggle.isOn, toggle);
        }
    }

    private void ToggleSprite(bool isOn, Toggle toggle) {
        toggle.image.sprite = isOn ? imgToggleOn : imgToggleOff;
    }
}
