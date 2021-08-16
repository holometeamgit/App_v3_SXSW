using UnityEngine;

namespace Beem.Extenject.Permissions {
    /// <summary>
    /// Permission for iOS
    /// </summary>
    public class iOSCameraPermission : ICameraPermission {

        public bool HasCameraAccess => Application.HasUserAuthorization(UserAuthorization.WebCam);

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
            Application.RequestUserAuthorization(UserAuthorization.WebCam);
        }

    }
}
