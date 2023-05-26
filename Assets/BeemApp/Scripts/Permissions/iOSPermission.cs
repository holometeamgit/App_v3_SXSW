using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

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

            //need because if user close the app before cancel or confirm camera we will have problem with mic. Because we will not ask permission
            bool resultPermission = true;

            foreach (var item in devicePermissions) {
                resultPermission = resultPermission && Application.HasUserAuthorization(permissions[item]);
            }

            return resultPermission;
        }

        public void RequestAccess(DevicePermissions[] devicePermissions, Action onSuccessed, Action onFailed, Action onRequestCompleted) {
            RequestAccessAsync(devicePermissions, onSuccessed, onFailed, onRequestCompleted);
        }

        private async void RequestAccessAsync(DevicePermissions[] devicePermissions, Action onSuccessed, Action onFailed, Action onRequestCompleted) {

            foreach (var item in devicePermissions) {
                if (!Application.HasUserAuthorization(permissions[item]))
                    await Application.RequestUserAuthorization(permissions[item]);
                if (!Application.HasUserAuthorization(permissions[item])) {
                    onFailed?.Invoke();
                    return;
                }
            }

            onRequestCompleted?.Invoke();
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
