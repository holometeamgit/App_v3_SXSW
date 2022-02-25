using Beem.Permissions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// This class is responsible for running the open home screen menu overlay
/// </summary>
public class PnlOpenHomeMenu : MonoBehaviour {

    [SerializeField]
    private UnityEvent OnShowCanvas;

    [SerializeField]
    private UnityEvent OnHideCanvas;

    [SerializeField]
    private Button btnOnEnableInvoke;

    [SerializeField]
    private Image imgPermissionRequired;

    [SerializeField]
    private ScrollRectSnapButtonHorz scrollRectSnapButtonHorz;

    private PermissionController permissionController = new PermissionController();

    private void OnEnable() {
        btnOnEnableInvoke?.onClick?.Invoke();
        permissionController.CheckCameraMicAccess(() => imgPermissionRequired.gameObject.SetActive(false), () => imgPermissionRequired.gameObject.SetActive(true));
        OnShowCanvas?.Invoke();
    }

    private void OnDisable() {
        OnHideCanvas?.Invoke();
    }


}
