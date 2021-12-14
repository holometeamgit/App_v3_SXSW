using UnityEngine;

namespace Beem.Permissions {

    /// <summary>
    /// Controllers for Different permissions
    /// </summary>
    public class PermissionController : MonoBehaviour {

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

        public bool CheckCameraMicAccess() {
            return CheckCameraAccess() && CheckMicAccess();
        }

        /// <summary>
        /// Check Camera Access
        /// </summary>
        /// <returns></returns>
        public bool CheckCameraAccess() {
            if (_permissionGranter.HasCameraAccess)
                return true;

            if (!CameraRequestComplete) {
                _permissionGranter.RequestCameraAccess();
                CameraRequestComplete = true;
            } else {
                OpenNotification(CAMERA_ACCESS);
            }

            return false;
        }

        /// <summary>
        /// Check Microphone Access
        /// </summary>
        /// <returns></returns>

        public bool CheckMicAccess() {
            if (_permissionGranter.HasMicAccess)
                return true;

            if (!MicRequestComplete) {
                _permissionGranter.RequestMicAccess();
                MicRequestComplete = true;
            } else {
                OpenNotification(MICROPHONE_ACCESS);
            }
            return false;
        }

        private void OpenNotification(string accessName) {
            WarningConstructor.ActivateDoubleButton(accessName + " access Required!",
                      "Please enable " + accessName + " access to use this app",
                      "Settings",
                      "Cancel",
                      () => OpenSettings(),
                      () => CloseNotification());
        }

        private void OpenSettings() {
            _permissionGranter.RequestSettings();
        }

        private void CloseNotification() {
            WarningConstructor.Deactivate();
        }
    }
}