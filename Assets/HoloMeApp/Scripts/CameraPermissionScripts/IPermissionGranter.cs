public interface IPermissionGranter
{
    bool HasCameraAccess { get; }
    bool MicAccessAvailable { get; }
    bool WriteAccessAvailable { get; }

    bool MicRequestComplete { get; }
    bool WriteRequestComplete { get; }
    bool CameraRequestComplete { get; }

    void RequestMicAccess();
    void RequestWriteAccess();
    void RequestCameraAccess();
}
