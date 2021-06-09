using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleTextSwap : MonoBehaviour {
    [SerializeField]
    TextMeshProUGUI textComponentReference;

    [SerializeField]
    string txtToggledOn;

    [SerializeField]
    string txtToggledOff;

    private void Awake() {

        if (textComponentReference == null) {
            HelperFunctions.DevLogError(nameof(ToggleTextSwap) + " was missing text component assignment, please assign for the component to work");
        } else {
            var toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(x => { ToggleText(x); });
            ToggleText(toggle.isOn);
        }
    }
    private void ToggleText(bool isOn) {
        textComponentReference.text = isOn ? txtToggledOn : txtToggledOff;
    }
}
