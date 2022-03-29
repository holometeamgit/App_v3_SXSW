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
            foreach (var item in devicePermissions) {
                if (!Permission.HasUserAuthorizedPermission(permissions[item])) {
                    return false;
                }
            }

            return true;
        }

        public void RequestAccess(DevicePermissions[] devicePermissions, Action onSuccessed, Action onFailed) {
            RequestAccessAsync(devicePermissions, onSuccessed, onFailed);
        }

        private async void RequestAccessAsync(DevicePermissions[] devicePermissions, Action onSuccessed, Action onFailed) {

            string[] tempPermissions = new string[devicePermissions.Length];
            for (int i = 0; i < devicePermissions.Length; i++) {
                tempPermissions[i] = permissions[devicePermissions[i]];
            }

            Permission.RequestUserPermissions(tempPermissions);
            await Task.Delay(DELAY);
            if (!HasAccesses(devicePermissions)) {
                onFailed?.Invoke();
                return;
            }

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
