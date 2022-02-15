using Beem.Permissions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PnlOpenHomeMenu : MonoBehaviour {
    [SerializeField]
    public UnityEvent OnEnabled;
    
    [SerializeField]
    public UnityEvent OnDisabled;
    
    [SerializeField]
    public Button btnOnEnableInvoke;
    
    [SerializeField]
    Canvas panelCanvas;
    
    [SerializeField]
    ScrollRectSnapButtonHorz scrollRectSnapButtonHorz;

    PermissionController permissionController = new PermissionController();

    private void OnEnable() {
        btnOnEnableInvoke?.onClick?.Invoke();
    }

    /// <summary>
    /// Enables the panel's canvas and fire enable events
    /// </summary>
    public void Activate() {
        OnEnabled?.Invoke();
        panelCanvas.enabled = true;
        //scrollRectSnapButtonHorz.ReactivateCurrentIndex();
    }

    /// <summary>
    /// Hide the panel but keep it activated
    /// </summary>
    public void HideCanvas() {
        panelCanvas.enabled = false;
    }

    public void ShowCanvas() {
        permissionController.CheckCameraMicAccess(() => { }, () => { });
        panelCanvas.enabled = true;
    }

    private void OnDisable() {
        //RoomTutorialConstructor.OnActivated?.Invoke(false);
        //StreamOverlayConstructor.onDeactivate?.Invoke();
        OnDisabled?.Invoke();
        StreamOverlayConstructor.onDeactivate?.Invoke();
    }
}
