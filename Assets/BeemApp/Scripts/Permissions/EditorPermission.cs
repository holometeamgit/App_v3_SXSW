using System;
using UnityEngine;

namespace Beem.Permissions {
    /// <summary>
    /// Permission for Editor
    /// </summary>
    public class EditorPermission : IPermissionGranter {
        public bool HasAccess(DevicePermissions devicePermission) {
            return true;
        }

        public bool HasAccesses(DevicePermissions[] devicePermissions) {
            return true;
        }

        public void RequestAccess(DevicePermissions[] devicePermissions, Action onSuccessed, Action onFailed, Action onRequestCompleted) {
            foreach (var item in devicePermissions) {
                Debug.LogError($"{nameof(EditorPermission)} Requested {item} Access Editor");
            }

            onRequestCompleted?.Invoke();
        }

        public void RequestSettings() {
            Debug.LogError($"{nameof(EditorPermission)} Requested Settings Editor");
        }
    }
}
