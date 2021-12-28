using System;
using UnityEngine;

namespace Beem.Permissions {
    /// <summary>
    /// Permission for Editor
    /// </summary>
    public class EditorPermission : IPermissionGranter {
        public bool HasCameraAccess => true;

        public bool HasMicAccess => true;

        public void RequestCameraAccess(Action onSuccessed, Action onFailed) {
            Debug.LogError($"{nameof(EditorPermission)} Requested Camera Access Editor");
        }

        public void RequestMicAccess(Action onSuccessed, Action onFailed) {
            Debug.LogError($"{nameof(EditorPermission)} Requested Mic Access Editor");
        }

        public void RequestSettings() {
            Debug.LogError($"{nameof(EditorPermission)} Requested Settings Editor");
        }
    }
}
