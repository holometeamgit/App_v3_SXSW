using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Settings Btn
/// </summary>
public class SettingsBtn : MonoBehaviour {
    [SerializeField]
    private bool isOpened;

    /// <summary>
    /// On Click Btn
    /// </summary>
    public void OnClick() {
        SettingsConstructor.OnActivated?.Invoke(isOpened);
    }
}
