
namespace Beem.Extenject.Permissions {
    /// <summary>
    /// Permission for Camera
    /// </summary>
    public interface ICameraPermission {
        string CameraKey { get; }
        bool HasCameraAccess { get; }
        void RequestCameraAccess();
        bool CameraRequestComplete { get; set; }
    }
}
