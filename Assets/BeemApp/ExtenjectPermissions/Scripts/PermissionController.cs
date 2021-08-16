using Beem.Extenject.UI;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Permissions {

    /// <summary>
    /// Controllers for Different permissions
    /// </summary>
    public class PermissionController {

        public PermissionController(WindowObject windowObject) {
            _windowObject = windowObject;
        }

        private IPermissionGranter _permissionGranter;
        private WindowController _windowController;
        private WindowObject _windowObject;

        [Inject]
        public void Construct(IPermissionGranter permissionGranter, WindowController windowController) {
            _permissionGranter = permissionGranter;
            _windowController = windowController;
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

            if (!_permissionGranter.CameraRequestComplete) {
                _permissionGranter.RequestCameraAccess();
                _permissionGranter.CameraRequestComplete = true;
            } else {
                OpenNotification(_permissionGranter.CameraKey);
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

            if (!_permissionGranter.MicRequestComplete) {
                _permissionGranter.RequestMicAccess();
                _permissionGranter.MicRequestComplete = true;
            } else {
                OpenNotification(_permissionGranter.MicKey);
            }
            return false;
        }

        private void OpenNotification(string accessName) {
            GeneralPopUpData generalPopUpData = new GeneralPopUpData(
                accessName + " access Required!",
                "Please enable " + accessName + " access to use this app",
                new GeneralPopUpData.ButtonData("Cancel", CloseNotification),
                new GeneralPopUpData.ButtonData("Settings", OpenSettings)
                );

            WindowSignal windowSignal = new WindowSignal(WindowSignalsType.OpenPopup, _windowObject);
            _windowController.OnCalledSignal(windowSignal, generalPopUpData);
        }

        private void OpenSettings() {
            _permissionGranter.RequestSettings();
        }

        private void CloseNotification() {
            WindowSignal windowSignal = new WindowSignal(WindowSignalsType.CloseWindow, _windowObject);
            _windowController.OnCalledSignal(windowSignal);
        }
    }
}