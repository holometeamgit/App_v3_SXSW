
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
        /// Has Access
        /// </summary>
        bool HasAccess(DevicePermissions devicePermission);

        /// <summary>
        /// Has Accesses
        /// </summary>
        bool HasAccesses(DevicePermissions[] devicePermissions);

        /// <summary>
        /// Request Access
        /// </summary>
        /// <param name="onSuccessed"></param>
        /// <param name="onFailed"></param>
        void RequestAccess(DevicePermissions[] devicePermissions, Action onSuccessed, Action onFailed);
    }
}
