using UnityEngine;

namespace Beem.Extenject.Permissions {
    /// <summary>
    /// Permission for Editor
    /// </summary>
    public class EditorCameraPermission : ICameraPermission {
        public bool HasCameraAccess => false;

        public string CameraKey => "Camera";

        public bool CameraRequestComplete {
            get {
                return true;
            }
            set { }
        }

        public void RequestCameraAccess() {
            Debug.LogError($"{nameof(EditorPermission)} Requested " + CameraKey + " Access Editor");
        }

    }
}
