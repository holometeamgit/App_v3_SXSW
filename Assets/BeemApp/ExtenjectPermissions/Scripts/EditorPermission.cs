using UnityEngine;

namespace Beem.Extenject.Permissions {
    /// <summary>
    /// Permission for Editor
    /// </summary>
    public class EditorPermission : IPermissionGranter {
        public bool HasCameraAccess => false;

        public bool HasMicAccess => false;

        public string CameraKey => "Camera";

        public string MicKey => "Microphone";

        public bool CameraRequestComplete {
            get {
                return true;
            }
            set { }
        }
        public bool MicRequestComplete {
            get {
                return true;
            }
            set { }
        }

        public void RequestCameraAccess() {
            Debug.LogError($"{nameof(EditorPermission)} Requested Camera Access Editor");
        }

        public void RequestMicAccess() {
            Debug.LogError($"{nameof(EditorPermission)} Requested Mic Access Editor");
        }

        public void RequestSettings() {
            Debug.LogError($"{nameof(EditorPermission)} Requested Settings Editor");
        }
    }
}
