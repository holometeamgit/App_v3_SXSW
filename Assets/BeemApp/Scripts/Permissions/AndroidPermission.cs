using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;

namespace Beem.Permissions {

    /// <summary>
    /// Permission for Android
    /// </summary>
    public class AndroidPermission : IPermissionGranter {

        private Dictionary<DevicePermissions, string> permissions = new Dictionary<DevicePermissions, string>() {
            { DevicePermissions.Camera, Permission.Camera },
            { DevicePermissions.Microphone, Permission.Microphone }
        };

        private const int DELAY = 3000;

        public bool HasAccess(DevicePermissions devicePermission) {
            return Permission.HasUserAuthorizedPermission(permissions[devicePermission]);
        }

        public bool HasAccesses(DevicePermissions[] devicePermissions) {
            //need because if user close the app before cancel or confirm camera we will have problem with mic. Because we will not ask permission
            bool resultPermission = true;
            foreach (var item in devicePermissions) {
                resultPermission = resultPermission && Permission.HasUserAuthorizedPermission(permissions[item]);
            }

            return resultPermission;
        }

        public void RequestAccess(DevicePermissions[] devicePermissions, Action onSuccessed, Action onFailed, Action onRequestCompleted) {
            RequestAccessAsync(devicePermissions, onSuccessed, onFailed, onRequestCompleted);
        }

        private async void RequestAccessAsync(DevicePermissions[] devicePermissions, Action onSuccessed, Action onFailed, Action onRequestCompleted) {

            string[] tempPermissions = new string[devicePermissions.Length];
            for (int i = 0; i < devicePermissions.Length; i++) {
                tempPermissions[i] = permissions[devicePermissions[i]];
            }

            if (!HasAccesses(devicePermissions))
                Permission.RequestUserPermissions(tempPermissions);
            await Task.Delay(DELAY);
            if (!HasAccesses(devicePermissions)) {
                onFailed?.Invoke();
                return;
            }

            onRequestCompleted?.Invoke();
            onSuccessed?.Invoke();
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
    }
}
