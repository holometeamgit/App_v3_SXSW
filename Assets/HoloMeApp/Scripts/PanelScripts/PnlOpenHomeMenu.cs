using Beem.Permissions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

    public void ShowCanvas() {
        permissionController.CheckCameraMicAccess(() => imgPermissionRequired.gameObject.SetActive(false), () => imgPermissionRequired.gameObject.SetActive(true));
        panelCanvas.enabled = true;
        OnShowCanvas?.Invoke();
    }

    private void OnDisable() {
        //RoomTutorialConstructor.OnActivated?.Invoke(false);
        //StreamOverlayConstructor.onDeactivate?.Invoke();
        StreamOverlayConstructor.onDeactivate?.Invoke();
    }
}
