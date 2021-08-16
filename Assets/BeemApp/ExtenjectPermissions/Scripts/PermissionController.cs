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

        private ISettingsPermission _settingsPermission;
        private IMicrophonePermission _microphonePermission;
        private ICameraPermission _cameraPermission;
        private WindowController _windowController;
        private WindowObject _windowObject;

        [Inject]
        public void Construct(ISettingsPermission settingsPermission, ICameraPermission cameraPermission, IMicrophonePermission microphonePermission, WindowController windowController) {
            _cameraPermission = cameraPermission;
            _microphonePermission = microphonePermission;
            _settingsPermission = settingsPermission;
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
            if (_cameraPermission.HasCameraAccess)
                return true;

            if (!_cameraPermission.CameraRequestComplete) {
                _cameraPermission.RequestCameraAccess();
                _cameraPermission.CameraRequestComplete = true;
            } else {
                OpenNotification(_cameraPermission.CameraKey);
            }

            return false;
        }

        /// <summary>
        /// Check Microphone Access
        /// </summary>
        /// <returns></returns>

        public bool CheckMicAccess() {
            if (_microphonePermission.HasMicAccess)
                return true;

            if (!_microphonePermission.MicRequestComplete) {
                _microphonePermission.RequestMicAccess();
                _microphonePermission.MicRequestComplete = true;
            } else {
                OpenNotification(_microphonePermission.MicKey);
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
            _settingsPermission.RequestSettings();
        }

        private void CloseNotification() {
            WindowSignal windowSignal = new WindowSignal(WindowSignalsType.CloseWindow, _windowObject);
            _windowController.OnCalledSignal(windowSignal);
        }
    }
}