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
        _cancelTokenSource = new CancellationTokenSource();
        RecheckPermission();
        OnShowCanvas?.Invoke();
    }

    private async void RecheckPermission() {

        Debug.LogError($"HasCameraMicAccess = {_permissionController.HasCameraMicAccess}");

        _permissionRequired.SetActive(!_permissionController.HasCameraMicAccess);

        CancellationToken cancellationToken = _cancelTokenSource.Token;

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
