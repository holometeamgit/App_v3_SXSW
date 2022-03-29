using System;
using UnityEngine;

/// <summary>
/// Use this class to activate info popup
/// </summary>
public class InfoPopupConstructor : MonoBehaviour {
    [SerializeField]
    private PnlInfoPopup pnlInfoPopup;

    public static Action<string, bool, PnlInfoPopupColour> onActivate = delegate { };
    public static Action<string, string, PnlInfoPopupColour> onActivateAsMessage = delegate { };

    private void OnEnable() {
        onActivate += pnlInfoPopup.Activate;
        onActivateAsMessage += pnlInfoPopup.ActivateAsMessage;
    }

    private void OnDisable() {
        onActivate -= pnlInfoPopup.Activate;
        onActivateAsMessage -= pnlInfoPopup.ActivateAsMessage;
    }

}
