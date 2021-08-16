using UnityEngine;

namespace Beem.Extenject.Permissions {
    /// <summary>
    /// Permission for iOS
    /// </summary>
    public class iOSPermission : IPermissionGranter {

        public bool HasMicAccess => Application.HasUserAuthorization(UserAuthorization.Microphone);

        public bool HasCameraAccess => Application.HasUserAuthorization(UserAuthorization.WebCam);

        public bool CameraRequestComplete {
            get {
                return PlayerPrefs.GetString("Access for " + CameraKey, "false") == "true";
            }
            set {
                PlayerPrefs.SetString("Access for " + CameraKey, value ? "true" : "false");
            }
        }

        public bool MicRequestComplete {
            get {
                return PlayerPrefs.GetString("Access for " + MicKey, "false") == "true";
            }
            set {
                PlayerPrefs.SetString("Access for " + MicKey, value ? "true" : "false");
            }
        }
        public string CameraKey => "Camera";

        public string MicKey => "Microphone";

        public void RequestCameraAccess() {
            Application.RequestUserAuthorization(UserAuthorization.WebCam);
        }

        public void RequestMicAccess() {
            Application.RequestUserAuthorization(UserAuthorization.Microphone);
        }

        public void RequestSettings() {
#if UNITY_IOS && !UNITY_EDITOR
        string url = iOSSettingsOpenerBindings.GetSettingsURL();
        Application.OpenURL(url);
#endif
        }

    }
}
