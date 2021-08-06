using UnityEngine;
using UnityEngine.Android;

namespace Beem.Permissions {

    /// <summary>
    /// Permission for Android
    /// </summary>
    public class AndroidPermission : IPermissionGranter {

        public bool HasCameraAccess => Permission.HasUserAuthorizedPermission(Permission.Camera);
        public bool HasMicAccess => Permission.HasUserAuthorizedPermission(Permission.Microphone);

        public void RequestMicAccess() {
            Permission.RequestUserPermission(Permission.Microphone);
        }

        public void RequestCameraAccess() {
            Permission.RequestUserPermission(Permission.Camera);
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
