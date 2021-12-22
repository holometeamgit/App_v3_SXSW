
using System;

namespace Beem.Permissions {
    /// <summary>
    /// Permission for Camera
    /// </summary>
    public interface ICameraPermission {

        /// <summary>
        /// Has Camera Access
        /// </summary>
        bool HasCameraAccess { get; }

        /// <summary>
        /// Request Camera Access
        /// </summary>
        /// <param name="onSuccessed"></param>
        /// <param name="onFailed"></param>
        void RequestCameraAccess(Action onSuccessed, Action onFailed);
    }
}
