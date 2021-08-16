using UnityEngine;
using UnityEngine.Android;

namespace Beem.Extenject.Permissions {

    /// <summary>
    /// Permission for Android
    /// </summary>
    public class AndroidCameraPermission : ICameraPermission {

        public bool HasCameraAccess => Permission.HasUserAuthorizedPermission(Permission.Camera);

        public bool CameraRequestComplete {
            get {
                return PlayerPrefs.GetString("Access for " + CameraKey, "false") == "true";
            }
            set {
                PlayerPrefs.SetString("Access for " + CameraKey, value ? "true" : "false");
            }
        }

        public string CameraKey => "Camera";

        public void RequestCameraAccess() {
            Permission.RequestUserPermission(Permission.Camera);
        }
    }
}
