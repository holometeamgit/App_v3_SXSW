using System;
using UnityEngine;

/// <summary>
/// Constructor for Menu window
/// </summary>
public class MenuConstructor : WindowConstructor {
    public static Action<bool> OnActivated = delegate { };
    public static Action<bool> OnActivateCanvas = delegate { };

    [SerializeField]
    private PnlOpenHomeMenu pnlOpenHomeMenu;

    protected void OnEnable() {
        OnActivated += Activate;
        OnActivateCanvas += ActivateCanvas;
    }

    protected void OnDisable() {
        OnActivated -= Activate;
        OnActivateCanvas -= ActivateCanvas;
    }

    /// <summary>
    /// Enable of disable the canvas for this menu
    /// </summary>
    public void ActivateCanvas(bool status) {
        if (!status)
            pnlOpenHomeMenu.HideCanvas();
        else
            pnlOpenHomeMenu.ShowCanvas();
    }

    protected void Activate(bool status) {
        _window.SetActive(status);
        ActivateCanvas(status);
    }
}
