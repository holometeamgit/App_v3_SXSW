
namespace Beem.Permissions {
    /// <summary>
    /// Permission for Camera
    /// </summary>
    public interface ICameraPermission {
        bool HasCameraAccess { get; }
        void RequestCameraAccess();
    }
}
