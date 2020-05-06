using UnityEngine;

public class PermissionGranter : MonoBehaviour, IPermissionGranter
{
    IPermissionGranter permissionGranter;

    public bool MicAccessAvailable => permissionGranter.MicAccessAvailable;
    public bool WriteAccessAvailable => permissionGranter.WriteAccessAvailable;
    public bool MicRequestComplete => permissionGranter.MicRequestComplete;
    public bool WriteRequestComplete => permissionGranter.WriteRequestComplete;
    public bool HasCameraAccess => permissionGranter.HasCameraAccess;

    void Awake()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            permissionGranter = gameObject.AddComponent<PnlCameraAccessCheckAndroid>();
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            permissionGranter = gameObject.AddComponent<PnlCameraAccessCheckiOS>();
        }
        else
        {
            permissionGranter = gameObject.AddComponent<PnlCameraAccessCheckEditor>();
        }
    }

    public void RequestCameraAccess()
    {
        if (!permissionGranter.HasCameraAccess)
        {
            permissionGranter.RequestCameraAccess();
        }
    }

    public void RequestMicAccess()
    {
        permissionGranter.RequestMicAccess();
    }

    public void RequestWriteAccess()
    {
        permissionGranter.RequestWriteAccess();
    }
}
