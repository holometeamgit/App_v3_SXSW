using UnityEngine;

/// <summary>
/// Controllers for Different permissions
/// </summary>
public class PermissionController : MonoBehaviour {

    [SerializeField]
    private PnlGenericError pnlGenericError;

    public IPermissionGranter PermissionGranter {
        get {
            return _permissionGranter;
        }
    }

    private IPermissionGranter _permissionGranter;

    private void Awake() {
        if (Application.platform == RuntimePlatform.Android) {
            _permissionGranter = new GranterForAndroid();
        } else if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _permissionGranter = new GranterForiOS();
        } else {
            _permissionGranter = new GranterForEditor();
        }
    }

    /// <summary>
    /// Check Camera and Mic Access
    /// </summary>
    /// <returns></returns>

    public bool CheckCameraMicAccess() {
        return CheckCameraAccess() && CheckMicAccess();
    }

    /// <summary>
    /// Check Camera Access
    /// </summary>
    /// <returns></returns>
    public bool CheckCameraAccess() {
        if (_permissionGranter.HasCameraAccess)
            return true;

        if (!_permissionGranter.CameraRequestComplete) {
            _permissionGranter.RequestCameraAccess();
        } else {
            pnlGenericError.ActivateDoubleButton("Camera Access Required!",
            "Please enable camera access to use this app",
            "Settings",
            "Cancel",
            () => _permissionGranter.RequestSettings(),
            () => pnlGenericError.gameObject.SetActive(false));
        }

        return false;
    }

    /// <summary>
    /// Check Microphone Access
    /// </summary>
    /// <returns></returns>

    public bool CheckMicAccess() {
        if (_permissionGranter.HasMicAccess)
            return true;

        if (!_permissionGranter.MicRequestComplete) {
            _permissionGranter.RequestMicAccess();
        } else {
            pnlGenericError.ActivateDoubleButton("Mic Access Required!",
                "Please enable mic access to use this app",
                "Settings",
                "Cancel",
                () => _permissionGranter.RequestSettings(),
                () => pnlGenericError.gameObject.SetActive(false));
        }
        return false;
    }
}
