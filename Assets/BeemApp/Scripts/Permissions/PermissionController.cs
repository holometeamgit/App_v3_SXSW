using NatSuite.Devices;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Beem.Permissions {

    /// <summary>
    /// Controllers for Different permissions
    /// </summary>
    public class PermissionController : MonoBehaviour {

        [SerializeField]
        private bool debugCameraAccess;

        [SerializeField]
        private bool debugMicAccess;

        public IPermissionGranter PermissionGranter {
            get {
                return _permissionGranter;
            }
        }

        private IPermissionGranter _permissionGranter;

        public bool MicRequestComplete {
            get {
                return PlayerPrefs.GetString("Access for " + MICROPHONE_ACCESS, "false") == "true";
            }
            private set {
                PlayerPrefs.SetString("Access for " + MICROPHONE_ACCESS, value ? "true" : "false");
            }
        }
        public bool CameraRequestComplete {
            get {
                return PlayerPrefs.GetString("Access for " + CAMERA_ACCESS, "false") == "true";
            }
            private set {
                PlayerPrefs.SetString("Access for " + CAMERA_ACCESS, value ? "true" : "false");
            }
        }

        private const string CAMERA_ACCESS = "Camera";
        private const string MICROPHONE_ACCESS = "Microphone";

        private const int DELAY = 3000;

        private void Awake() {
            if (Application.platform == RuntimePlatform.Android) {
                _permissionGranter = new AndroidPermission();
            } else if (Application.platform == RuntimePlatform.IPhonePlayer) {
                _permissionGranter = new iOSPermission();
            } else {
                _permissionGranter = new EditorPermission();
            }
        }

        /// <summary>
        /// Check Camera and Mic Access
        /// </summary>
        /// <returns></returns>

        public void CheckCameraMicAccess(Action onSuccessed, Action onFailed = null) {
            CheckCameraAccess(() => CheckMicAccess(onSuccessed, onFailed), onFailed); ;
        }

        /// <summary>
        /// Check Camera Access
        /// </summary>
        /// <returns></returns>
        public void CheckCameraAccess(Action onSuccessed, Action onFailed = null) {

#if UNITY_EDITOR
            HelperFunctions.DevLogError($"debugCameraAccess = {debugCameraAccess}");
            if (!debugCameraAccess) {
                OpenNotification(CAMERA_ACCESS, () => {
                    HelperFunctions.DevLogError($"debugCameraAccess = {debugCameraAccess}");
                    if (debugCameraAccess) {
                        onSuccessed?.Invoke();
                    } else {
                        onFailed?.Invoke();
                    }
                }); ;
            } else {
                onSuccessed.Invoke();
            }
#else

            if (_permissionGranter.HasCameraAccess) {
                onSuccessed.Invoke();
                return;
            }

            if (!CameraRequestComplete) {
                _permissionGranter.RequestCameraAccess(onSuccessed, onFailed);
                CameraRequestComplete = true;
                return;
            }


            OpenNotification(CAMERA_ACCESS, () => {
                if (_permissionGranter.HasCameraAccess) {
                    onSuccessed?.Invoke();
                } else {
                    onFailed?.Invoke();
                }
            }); ;
#endif
        }

        /// <summary>
        /// Check Microphone Access
        /// </summary>
        /// <returns></returns>

        public void CheckMicAccess(Action onSuccessed, Action onFailed = null) {

#if UNITY_EDITOR
            HelperFunctions.DevLogError($"debugMicAccess = {debugMicAccess}");
            if (!debugMicAccess) {
                OpenNotification(MICROPHONE_ACCESS, () => {
                    HelperFunctions.DevLogError($"debugMicAccess = {debugMicAccess}");
                    if (debugMicAccess) {
                        onSuccessed?.Invoke();
                    } else {
                        onFailed?.Invoke();
                    }
                });
            } else {
                onSuccessed.Invoke();
            }
#else

            if (_permissionGranter.HasMicAccess) {
                onSuccessed.Invoke();
                return;
            }

            if (!MicRequestComplete) {
                _permissionGranter.RequestMicAccess(onSuccessed, onFailed);
                MicRequestComplete = true;
                return;
            }


            OpenNotification(MICROPHONE_ACCESS, () => {
                if (_permissionGranter.HasMicAccess) {
                    onSuccessed?.Invoke();
                } else {
                    onFailed?.Invoke();
                }
            }); ;
#endif
        }

        private void OpenNotification(string accessName, Action onClosed) {
            WarningConstructor.ActivateDoubleButton(accessName + " access Required!",
                      "Please enable " + accessName + " access to use this app",
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