using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Settings Btn
/// </summary>
public class SettingsBtn : MonoBehaviour {
    [SerializeField]
    private bool isOpened;
    public void OnClick() {
        SettingsConstructor.OnActivated?.Invoke(isOpened);
    }
}
