using System;
using UnityEngine;

/// <summary>
/// Use this class to activate info popup
/// </summary>
public class InfoPopupConstructor : MonoBehaviour {
    [SerializeField]
    PnlInfoPopup pnlInfoPopup;

    public static Action<string, bool, Color> onActivate = delegate { };

    private void OnEnable() {
        onActivate += pnlInfoPopup.Activate;
    }

    private void OnDisable() {
        onActivate -= pnlInfoPopup.Activate;
    }

}
