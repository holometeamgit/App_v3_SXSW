using UnityEngine;

public class PermissionGranter : MonoBehaviour, IPermissionGranter {

    IPermissionGranter permissionGranter;

    public bool HasCameraAccess => permissionGranter.HasCameraAccess;
    public bool HasMicAccess => permissionGranter.HasMicAccess;
    public bool HasWriteAccess => permissionGranter.HasWriteAccess;

    public bool MicRequestComplete => permissionGranter.MicRequestComplete;
    public bool WriteRequestComplete => permissionGranter.WriteRequestComplete;
    public bool CameraRequestComplete => permissionGranter.CameraRequestComplete;


    public void RequestCameraAccess() {
        if (!permissionGranter.HasCameraAccess) {
            permissionGranter.RequestCameraAccess();
        }
    }

    public void RequestMicAccess() {
        permissionGranter.RequestMicAccess();
    }

    public void RequestSettings() {
        permissionGranter.RequestSettings();
    }

    public void RequestWriteAccess() {
        permissionGranter.RequestWriteAccess();
    }

    void Awake() {
        if (Application.platform == RuntimePlatform.Android) {
            permissionGranter = new GranterForAndroid();
        } else if (Application.platform == RuntimePlatform.IPhonePlayer) {
            permissionGranter = new GranterForiOS();
        } else {
            permissionGranter = new GranterForEditor();
        }
    }
}
