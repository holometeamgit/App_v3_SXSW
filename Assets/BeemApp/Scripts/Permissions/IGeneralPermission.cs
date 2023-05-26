
using System;

namespace Beem.Permissions {

    public enum DevicePermissions {
        Camera,
        Microphone
    }

    /// <summary>
    /// Permissions
    /// </summary>
    public interface IGeneralPermission {

        /// <summary>
        /// Check permission from device
        /// </summary>
        bool HasAccess(DevicePermissions devicePermission);

        /// <summary>
        /// Check permissions from device
        /// </summary>
        bool HasAccesses(DevicePermissions[] devicePermissions);

        /// <summary>
        /// Request Access
        /// </summary>
        /// <param name="onSuccessed"></param>
        /// <param name="onFailed"></param>
        /// <param name="onRequestCompleted">when all requests completed</param>
        void RequestAccess(DevicePermissions[] devicePermissions, Action onSuccessed, Action onFailed, Action onRequestCompleted);
    }
}
