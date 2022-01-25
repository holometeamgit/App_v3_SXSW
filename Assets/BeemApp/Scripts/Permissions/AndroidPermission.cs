using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;

namespace Beem.Permissions {

    /// <summary>
    /// Permission for Android
    /// </summary>
    public class AndroidPermission : IPermissionGranter, ISettingsPermission {

        public bool HasCameraAccess => Permission.HasUserAuthorizedPermission(Permission.Camera);
        public bool HasMicAccess => Permission.HasUserAuthorizedPermission(Permission.Microphone);

        private const string CAMERA_ACCESS = "Camera";
        private const string MICROPHONE_ACCESS = "Microphone";
        private const int DELAY = 3000;

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

        public void RequestMicAccess(Action onSuccessed, Action onFailed) {

            if (HasMicAccess) {
                onSuccessed.Invoke();
                return;
            }

            if (!MicRequestComplete) {
                var callbacks = new PermissionCallbacks();
                callbacks.PermissionDenied += (value) => { onFailed?.Invoke(); };
                callbacks.PermissionGranted += (value) => { onSuccessed?.Invoke(); };
                callbacks.PermissionDeniedAndDontAskAgain += (value) => { onFailed?.Invoke(); MicRequestComplete = true; };
                Permission.RequestUserPermission(Permission.Microphone, callbacks);
                return;
            }


            OpenNotification(MICROPHONE_ACCESS, () => {
                if (HasMicAccess) {
                    onSuccessed?.Invoke();
                } else {
                    onFailed?.Invoke();
                }
            });
        }

        public void RequestCameraAccess(Action onSuccessed, Action onFailed) {

            if (HasCameraAccess) {
                onSuccessed.Invoke();
                return;
            }

            if (!CameraRequestComplete) {
                var callbacks = new PermissionCallbacks();
                callbacks.PermissionDenied += (value) => { onFailed?.Invoke(); };
                callbacks.PermissionGranted += (value) => { onSuccessed?.Invoke(); };
                callbacks.PermissionDeniedAndDontAskAgain += (value) => { onFailed?.Invoke(); CameraRequestComplete = true; };
                Permission.RequestUserPermission(Permission.Camera, callbacks);
                return;
            }


            OpenNotification(CAMERA_ACCESS, () => {
                if (HasCameraAccess) {
                    onSuccessed?.Invoke();
                } else {
                    onFailed?.Invoke();
                }
            });
        }

        public void RequestSettings() {
#if UNITY_ANDROID
            try {
                using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity")) {
                    string packageName = currentActivityObject.Call<string>("getPackageName");

                    using (var uriClass = new AndroidJavaClass("android.net.Uri"))
                    using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
                    using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject)) {
                        intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                        intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                        currentActivityObject.Call("startActivity", intentObject);
                    }
                }
            } catch (System.Exception ex) {
                Debug.LogException(ex);
            }
#endif
        }

        private void OpenNotification(string accessName, Action onClosed) {
            WarningConstructor.ActivateDoubleButton(accessName + " access Required!",
                      "Please enable " + accessName + " access to use this app",
                      "Settings",
                      "Cancel",
                      () => {
                          RequestSettings();
                          RecheckAsync(onClosed);
                      });
        }

        private async void RecheckAsync(Action onClosed) {
            await Task.Delay(DELAY);
            onClosed?.Invoke();
        }
    }
}
