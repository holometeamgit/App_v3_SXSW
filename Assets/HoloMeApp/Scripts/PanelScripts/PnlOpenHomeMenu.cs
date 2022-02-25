using Beem.Permissions;
using System.Threading;
using System.Threading.Tasks;
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

    private PermissionController _permissionController = new PermissionController();
    private CancellationTokenSource _cancelTokenSource;

    private const int DELAY = 10000;

    private void OnEnable() {
        btnOnEnableInvoke?.onClick?.Invoke();
        imgPermissionRequired.gameObject.SetActive(true);
        _cancelTokenSource = new CancellationTokenSource();
        _permissionController.CheckCameraMicAccess(() => imgPermissionRequired.gameObject.SetActive(false), () => RecheckPermission());
        OnShowCanvas?.Invoke();
    }

    private async void RecheckPermission() {
        CancellationToken cancellationToken = _cancelTokenSource.Token;
        if (!cancellationToken.IsCancellationRequested) {
            await Task.Delay(DELAY);
        }
        _permissionController.CheckCameraMicAccess(() => imgPermissionRequired.gameObject.SetActive(false), () => RecheckPermission());
    }

    private void OnDisable() {
        Cancel();
        OnHideCanvas?.Invoke();
    }

    /// <summary>
    /// Clear Info
    /// </summary>
    public void Cancel() {
        if (_cancelTokenSource != null) {
            _cancelTokenSource.Cancel();
            _cancelTokenSource = null;
        }
    }

}
