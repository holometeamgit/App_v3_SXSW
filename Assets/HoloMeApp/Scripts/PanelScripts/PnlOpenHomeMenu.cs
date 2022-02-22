using Beem.Permissions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// This class is responsible for running the open home screen menu overlay
/// </summary>
public class PnlOpenHomeMenu : MonoBehaviour {

    [SerializeField]
    public UnityEvent OnShowCanvas;

    [SerializeField]
    public UnityEvent OnHideCanvas;

    [SerializeField]
    public Button btnOnEnableInvoke;

    [SerializeField]
    public Image imgPermissionRequired;

    [SerializeField]
    private Canvas panelCanvas;

    [SerializeField]
    private ScrollRectSnapButtonHorz scrollRectSnapButtonHorz;

    private PermissionController permissionController = new PermissionController();

    private void OnEnable() {
        btnOnEnableInvoke?.onClick?.Invoke();
    }

    /// <summary>
    /// Enables the panel's canvas and fire enable events
    /// </summary>
    public void Activate() {
        panelCanvas.enabled = true;
        //scrollRectSnapButtonHorz.ReactivateCurrentIndex();
    }

    /// <summary>
    /// Hide the panel but keep it activated
    /// </summary>
    public void HideCanvas() {
        panelCanvas.enabled = false;
        OnHideCanvas?.Invoke();
    }

    /// <summary>
    /// Show the canvas, checks camera permissions before enabling
    /// </summary>
    public void ShowCanvas() {
        permissionController.CheckCameraMicAccess(() => imgPermissionRequired.gameObject.SetActive(false), () => imgPermissionRequired.gameObject.SetActive(true));
        panelCanvas.enabled = true;
        OnShowCanvas?.Invoke();
    }

    private void OnDisable() {
        StreamOverlayConstructor.onDeactivate?.Invoke();
    }
}
