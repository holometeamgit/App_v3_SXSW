using UnityEngine;
using UnityEngine.Android;

namespace Beem.Extenject.Permissions {

    /// <summary>
    /// Permission for Android
    /// </summary>
    public class AndroidPermission : ISettingsPermission {

        public void RequestSettings() {
#if UNITY_ANDROID && !UNITY_EDITOR
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
