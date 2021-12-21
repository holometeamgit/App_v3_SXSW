using System;
using System.Threading;
using System.Threading.Tasks;
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

        private CancellationTokenSource _showCancellationTokenSource;
        private CancellationToken _showCancellationToken;

        private const string CAMERA_ACCESS = "Camera";
        private const string MICROPHONE_ACCESS = "Microphone";
        private const int CHECK_COOLDOWN = 5000;

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

        private async Task CheckPermission(bool permission) {
            while (!permission) {
                if (_showCancellationToken.IsCancellationRequested) {
                    _showCancellationToken.ThrowIfCancellationRequested();
                }
                await Task.Delay(CHECK_COOLDOWN);
            }
        }

        private void OnPermissionCheck(bool permission, Action onSuccessed = null) {
            if (_showCancellationTokenSource != null) {
                _showCancellationTokenSource.Cancel();
                _showCancellationTokenSource.Dispose();
            }

            _showCancellationTokenSource = new CancellationTokenSource();
            _showCancellationToken = _showCancellationTokenSource.Token;


            TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            CheckPermission(permission).ContinueWith((task) => {

                if (task.IsCompleted) {
                    onSuccessed?.Invoke();
                } else {
                    OnPermissionCheck(permission, onSuccessed);
                }
            }, taskScheduler);
        }

        /// <summary>
        /// Check Camera Permission
        /// </summary>
        /// <param name="onSuccessed"></param>
        public void CheckCameraPermission(Action onSuccessed = null) {
            OnPermissionCheck(CheckCameraAccess(), onSuccessed);
        }

        /// <summary>
        /// Check Mic Permission
        /// </summary>
        /// <param name="onSuccessed"></param>
        public void CheckMicPermission(Action onSuccessed = null) {
            OnPermissionCheck(CheckMicAccess(), onSuccessed);
        }

        /// <summary>
        /// Check Camera and Mic Permission
        /// </summary>
        /// <param name="onSuccessed"></param>
        public void CheckCameraMicPermission(Action onSuccessed = null) {
            OnPermissionCheck(CheckCameraMicAccess(), onSuccessed);
        }
    }
}