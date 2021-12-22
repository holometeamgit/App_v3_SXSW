using System;
using UnityEngine;

namespace Beem.Permissions {
    /// <summary>
    /// Permission for iOS
    /// </summary>
    public class iOSPermission : IPermissionGranter {

        public bool HasMicAccess => Application.HasUserAuthorization(UserAuthorization.Microphone);

        public bool HasCameraAccess => Application.HasUserAuthorization(UserAuthorization.WebCam);

        public void RequestCameraAccess(Action onSuccessed, Action onFailed) {
            RequestCameraAccessAsync(onSuccessed, onFailed);
        }

        private async void RequestCameraAccessAsync(Action onSuccessed, Action onFailed) {
            await Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (HasCameraAccess) {
                onSuccessed?.Invoke();
            } else {
                onFailed?.Invoke();
            }
        }

        public void RequestMicAccess(Action onSuccessed, Action onFailed) {
            RequestMicAccessAsync(onSuccessed, onFailed);
        }

        private async void RequestMicAccessAsync(Action onSuccessed, Action onFailed) {
            await Application.RequestUserAuthorization(UserAuthorization.Microphone);
            if (HasMicAccess) {
                onSuccessed?.Invoke();
            } else {
                onFailed?.Invoke();
            }
        }

        public void RequestSettings() {
#if UNITY_IOS && !UNITY_EDITOR
        string url = iOSSettingsOpenerBindings.GetSettingsURL();
        Application.OpenURL(url);
#endif
        }

    }
}
