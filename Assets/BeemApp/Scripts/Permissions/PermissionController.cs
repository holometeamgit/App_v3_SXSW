using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Beem.Permissions {

    /// <summary>
    /// Controllers for Different permissions
    /// </summary>
    public class PermissionController {
        public IPermissionGranter PermissionGranter {
            get {
                return _permissionGranter;
            }
        }

        private IPermissionGranter _permissionGranter;

        public bool RequestComplete {
            get {
                return PlayerPrefs.GetString("RequestPermissionComplete", "false") == "true";
            }
            private set {
                PlayerPrefs.SetString("RequestPermissionComplete", value ? "true" : "false");
            }
        }

        private const int DELAY = 3000;

        public PermissionController() {
            if (Application.platform == RuntimePlatform.Android) {
                _permissionGranter = new AndroidPermission();
            } else if (Application.platform == RuntimePlatform.IPhonePlayer) {
                _permissionGranter = new iOSPermission();
            } else {
                _permissionGranter = new EditorPermission();
            }
        }

        public bool HasCameraMicAccess {
            get {
                return HasCameraAccess && HasMicAccess;
            }
        }

        public bool HasCameraAccess {
            get {
                return _permissionGranter.HasAccess(DevicePermissions.Camera);
            }
        }

        public bool HasMicAccess {
            get {
                return _permissionGranter.HasAccess(DevicePermissions.Microphone);
            }
        }

        /// <summary>
        /// Check Camera and Mic Access
        /// </summary>
        /// <returns></returns>

        public void CheckCameraMicAccess(Action onSuccessed, Action onFailed = null) {

            if (HasCameraMicAccess) {
                onSuccessed.Invoke();
                return;
            }

            if (!RequestComplete) {
                _permissionGranter.RequestAccess(new DevicePermissions[] { DevicePermissions.Camera, DevicePermissions.Microphone }, onSuccessed, onFailed);
                RequestComplete = true;
                return;
            }


            OpenNotification(DevicePermissions.Camera + " and " + DevicePermissions.Microphone, () => {
                if (HasCameraMicAccess) {
                    onSuccessed?.Invoke();
                } else {
                    onFailed?.Invoke();
                }
            }); ;

        }

        private void OpenNotification(string accessName, Action onClosed) {
            WarningConstructor.ActivateDoubleButton(accessName + " accesses Required!",
                      "Please enable " + accessName + " accesses to use this app",
                      "Settings",
                      "Cancel",
                      () => {
                          OpenSettings();
                          RecheckAsync(onClosed);
                      });
        }

        private async void RecheckAsync(Action onClosed) {
            await Task.Delay(DELAY);
            onClosed?.Invoke();
        }

        private void OpenSettings() {
            _permissionGranter.RequestSettings();
        }
    }
}