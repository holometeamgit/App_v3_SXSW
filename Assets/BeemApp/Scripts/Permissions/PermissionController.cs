using System;
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

        public PermissionController() {
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
            _permissionGranter.RequestCameraAccess(onSuccessed, onFailed);
        }

        /// <summary>
        /// Check Microphone Access
        /// </summary>
        /// <returns></returns>

        public void CheckMicAccess(Action onSuccessed, Action onFailed = null) {
            _permissionGranter.RequestMicAccess(onSuccessed, onFailed);
        }
    }
}