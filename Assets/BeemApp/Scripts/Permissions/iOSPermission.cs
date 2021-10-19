using UnityEngine;

namespace Beem.Permissions {
    /// <summary>
    /// Permission for iOS
    /// </summary>
    public class iOSPermission : IPermissionGranter {

        public bool HasMicAccess => Application.HasUserAuthorization(UserAuthorization.Microphone);

        public bool HasCameraAccess => Application.HasUserAuthorization(UserAuthorization.WebCam);

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
