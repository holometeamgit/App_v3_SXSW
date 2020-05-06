public interface IPermissionGranter
{
    void RequestMicAccess();
    void RequestWriteAccess();
    void RequestCameraAccess();

    bool HasCameraAccess { get; }
    bool MicAccessAvailable { get; }
    bool WriteAccessAvailable { get; }
    bool MicRequestComplete { get; }
    bool WriteRequestComplete { get; }
}
