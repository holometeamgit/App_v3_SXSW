using UnityEngine;

public class GranterForEditor : IPermissionGranter {
    public bool HasCameraAccess => true;

    public bool HasMicAccess => true;

    public bool HasWriteAccess => true;

    public bool MicRequestComplete => true;

    public bool WriteRequestComplete => true;

    public bool CameraRequestComplete => true;

    public void RequestCameraAccess() {
        Debug.LogError($"{nameof(GranterForEditor)} Requested Camera Access Editor");
    }

    public void RequestMicAccess() {
        Debug.LogError($"{nameof(GranterForEditor)} Requested Mic Access Editor");
    }

    public void RequestSettings() {
        Debug.LogError($"{nameof(GranterForEditor)} Requested Settings Editor");
    }

    public void RequestWriteAccess() {
        Debug.LogError($"{nameof(GranterForEditor)} Requested Write Access Editor");
    }
}
