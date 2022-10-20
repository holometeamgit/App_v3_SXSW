using UnityEngine;
using Zenject;

/// <summary>
/// Use this class to enable monobehaviours if they should be running for premium users only.
/// </summary>
public class PremiumComponentEnabler : MonoBehaviour {
    [SerializeField]
    private MonoBehaviour[] componentsToEnable;

    private BusinessProfileManager _businessProfileManager;

    [Inject]
    private void Construct(BusinessProfileManager businessProfileManager) {
        _businessProfileManager = businessProfileManager;
    }

    private void OnEnable() {
        foreach (MonoBehaviour component in componentsToEnable) {
            component.enabled = _businessProfileManager.IsBusinessProfile();
        }
    }
}
