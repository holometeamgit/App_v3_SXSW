public interface IPermissionGranter {
    bool HasCameraAccess { get; }
    bool HasMicAccess { get; }
    bool HasWriteAccess { get; }

    bool MicRequestComplete { get; }
    bool WriteRequestComplete { get; }
    bool CameraRequestComplete { get; }

    void RequestMicAccess();
    void RequestWriteAccess();
    void RequestCameraAccess();

    void RequestSettings();
}
