using System;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Permissions {
    /// <summary>
    /// Permission for iOS
    /// </summary>
    public class iOSPermission : IPermissionGranter {

        private Dictionary<DevicePermissions, UserAuthorization> permissions = new Dictionary<DevicePermissions, UserAuthorization>() {
            { DevicePermissions.Camera, UserAuthorization.WebCam },
            { DevicePermissions.Microphone, UserAuthorization.Microphone }
        };

        public bool HasAccess(DevicePermissions devicePermission) {
            return Application.HasUserAuthorization(permissions[devicePermission]);
        }

        public bool HasAccesses(DevicePermissions[] devicePermissions) {
            foreach (var item in devicePermissions) {
                if (!Application.HasUserAuthorization(permissions[item])) {
                    return false;
                }
            }

            return true;
        }

        public void RequestAccess(DevicePermissions[] devicePermissions, Action onSuccessed, Action onFailed) {
            RequestAccessAsync(devicePermissions, onSuccessed, onFailed);
        }


        private async void RequestAccessAsync(DevicePermissions[] devicePermissions, Action onSuccessed, Action onFailed) {

            foreach (var item in devicePermissions) {
                await Application.RequestUserAuthorization(permissions[item]);
                if (!Application.HasUserAuthorization(permissions[item])) {
                    onFailed?.Invoke();
                    return;
                }
            }

            onSuccessed?.Invoke();
        }

        public void RequestSettings() {
#if UNITY_IOS && !UNITY_EDITOR
        string url = iOSSettingsOpenerBindings.GetSettingsURL();
        Application.OpenURL(url);
#endif
        }
    }
}
