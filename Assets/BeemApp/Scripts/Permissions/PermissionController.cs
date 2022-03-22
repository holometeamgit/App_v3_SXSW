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
                return HasAccesses(new DevicePermissions[] { DevicePermissions.Camera, DevicePermissions.Microphone });
            }
        }

        public bool HasAccesses(DevicePermissions[] devicePermissions) {
            return _permissionGranter.HasAccesses(devicePermissions);
        }

        /// <summary>
        /// Check Camera and Mic Access
        /// </summary>
        /// <returns></returns>

        public void CheckCameraMicAccess(Action onSuccessed, Action onFailed = null) {
            CheckAccesses(new DevicePermissions[] { DevicePermissions.Camera, DevicePermissions.Microphone }, onSuccessed, onFailed);
        }

        /// <summary>
        /// Check Acceses
        /// </summary>
        /// <param name="devicePermissions"></param>
        /// <param name="onSuccessed"></param>
        /// <param name="onFailed"></param>
        public void CheckAccesses(DevicePermissions[] devicePermissions, Action onSuccessed, Action onFailed = null) {

            if (HasAccesses(devicePermissions)) {
                onSuccessed.Invoke();
                return;
            }

            if (!RequestComplete) {
                _permissionGranter.RequestAccess(devicePermissions, onSuccessed, onFailed);
                RequestComplete = true;
                return;
            }


            OpenNotification(AccessesStrings(devicePermissions), () => {
                if (HasAccesses(devicePermissions)) {
                    onSuccessed?.Invoke();
                } else {
                    onFailed?.Invoke();
                }
            }); ;

        }

        private string AccessesStrings(DevicePermissions[] devicePermissions) {
            switch (devicePermissions.Length) {
                case 0:
                    return string.Empty;
                case 1:
                    return devicePermissions[0].ToString();
                default:
                    string str = string.Empty;
                    for (int i = 0; i < devicePermissions.Length; i++) {
                        if (i < devicePermissions.Length - 2) {
                            str += devicePermissions[i] + ", ";
                        } else if (i == devicePermissions.Length - 2) {
                            str += devicePermissions[i] + " and ";
                        } else {
                            str += devicePermissions[i];
                        }
                    }
                    return str;
            }
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