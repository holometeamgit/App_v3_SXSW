using UnityEngine;

public class PnlCameraAccessCheckEditor : MonoBehaviour, IPermissionGranter
{
    public bool HasCameraAccess => true;

    public bool MicAccessAvailable => true;

    public bool WriteAccessAvailable => true;

    public bool MicRequestComplete => true;

    public bool WriteRequestComplete => true;

    public bool CameraRequestComplete => true;

    public void RequestCameraAccess()
    {
        Debug.Log($"{nameof(PnlCameraAccessCheckEditor)} Requested Camera Access Editor");
    }

    public void RequestMicAccess()
    {
        Debug.Log($"{nameof(PnlCameraAccessCheckEditor)} Requested Mic Access Editor");
    }

    public void RequestSettings()
    {
        Debug.Log($"{nameof(PnlCameraAccessCheckEditor)} Requested Settings Editor");
    }

    public void RequestWriteAccess()
    {
        Debug.Log($"{nameof(PnlCameraAccessCheckEditor)} Requested Write Access Editor");
    }
}
