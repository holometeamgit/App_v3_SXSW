using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// This is intended to enable or disable buttons in the open home menu based on business status. Room and Stadium
/// </summary>
public class PremiumButtonEnabler : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI _textToDisable;
    [SerializeField]
    private Button _buttonToToggle;

    private BusinessProfileManager _businessProfileManager;

    [Inject]
    private void Construct(BusinessProfileManager businessProfileManager) {
        _businessProfileManager = businessProfileManager;
    }

    private void OnEnable() {
        _buttonToToggle.interactable = _businessProfileManager.IsBusinessProfile();
        _textToDisable.gameObject.SetActive(_businessProfileManager.IsBusinessProfile());
    }
}
