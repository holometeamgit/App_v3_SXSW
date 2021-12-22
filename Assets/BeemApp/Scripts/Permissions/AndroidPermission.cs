using System;
using UnityEngine;
using UnityEngine.Android;

namespace Beem.Permissions {

    /// <summary>
    /// Permission for Android
    /// </summary>
    public class AndroidPermission : IPermissionGranter {

        public bool HasCameraAccess => Permission.HasUserAuthorizedPermission(Permission.Camera);
        public bool HasMicAccess => Permission.HasUserAuthorizedPermission(Permission.Microphone);

        public void RequestMicAccess(Action onSuccessed, Action onFailed) {
            var callbacks = new PermissionCallbacks();
            callbacks.PermissionDenied += (value) => { HelperFunctions.DevLogError($"{value} PermissionCallbacks_PermissionDenied"); onFailed?.Invoke(); };
            callbacks.PermissionGranted += (value) => { HelperFunctions.DevLogError($"{value} PermissionCallbacks_PermissionGranted"); onSuccessed?.Invoke(); };
            callbacks.PermissionDeniedAndDontAskAgain += (value) => { HelperFunctions.DevLogError($"{value} PermissionCallbacks_PermissionDeniedAndDontAskAgain"); onFailed?.Invoke(); };
            Permission.RequestUserPermission(Permission.Microphone, callbacks);
        }

        public void RequestCameraAccess(Action onSuccessed, Action onFailed) {
            var callbacks = new PermissionCallbacks();
            callbacks.PermissionDenied += (value) => { HelperFunctions.DevLogError($"{value} PermissionCallbacks_PermissionDenied"); onFailed?.Invoke(); };
            callbacks.PermissionGranted += (value) => { HelperFunctions.DevLogError($"{value} PermissionCallbacks_PermissionGranted"); onSuccessed?.Invoke(); };
            callbacks.PermissionDeniedAndDontAskAgain += (value) => { HelperFunctions.DevLogError($"{value} PermissionCallbacks_PermissionDeniedAndDontAskAgain"); onFailed?.Invoke(); };
            Permission.RequestUserPermission(Permission.Camera, callbacks);
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
