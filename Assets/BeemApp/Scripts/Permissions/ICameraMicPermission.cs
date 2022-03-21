
using System;

namespace Beem.Permissions {
    /// <summary>
    /// Permission for Camera
    /// </summary>
    public interface ICameraMicPermission {

        /// <summary>
        /// Has Camera Access
        /// </summary>
        bool HasCameraMicAccess { get; }

        /// <summary>
        /// Request Camera Access
        /// </summary>
        /// <param name="onSuccessed"></param>
        /// <param name="onFailed"></param>
        void RequestCameraMicAccess(Action onSuccessed, Action onFailed);
    }
}
