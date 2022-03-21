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
    private GameObject _permissionRequired;

    [SerializeField]
    private ScrollRectSnapButtonHorz scrollRectSnapButtonHorz;

    private PermissionController _permissionController = new PermissionController();
    private CancellationTokenSource _cancelTokenSource;

    private const int DELAY = 3000;

    private void OnEnable() {
        btnOnEnableInvoke?.onClick?.Invoke();
        RecheckPermission();
        OnShowCanvas?.Invoke();
    }

    private void RecheckPermission() {
        _permissionRequired.SetActive(!_permissionController.HasCameraMicAccess);

        _permissionController.CheckCameraMicAccess(OnSuccess, OnFailed);
    }

    private void OnSuccess() {
        _permissionRequired.SetActive(false);
    }

    private async void OnFailed() {
        _cancelTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = _cancelTokenSource.Token;
        _permissionRequired.SetActive(true);
        if (!(_permissionController.HasCameraMicAccess || cancellationToken.IsCancellationRequested)) {
            await Task.Delay(DELAY);
            RecheckPermission();
        }
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
