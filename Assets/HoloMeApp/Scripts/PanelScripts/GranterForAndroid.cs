using UnityEngine;
using UnityEngine.Android;

public class GranterForAndroid : IPermissionGranter {

    public bool HasCameraAccess => Permission.HasUserAuthorizedPermission(Permission.Camera);
    public bool HasMicAccess => Permission.HasUserAuthorizedPermission(Permission.Microphone);
    public bool HasWriteAccess => Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite);

    public bool MicRequestComplete {
        get {
            return PlayerPrefs.GetString("Access for " + MICROPHONE_ACCESS, "false") == "true";
        }
        private set {
            PlayerPrefs.SetString("Access for " + MICROPHONE_ACCESS, value ? "true" : "false");
        }
    }
    public bool WriteRequestComplete {
        get {
            return PlayerPrefs.GetString("Access for " + WRITE_ACCESS, "false") == "true";
        }
        private set {
            PlayerPrefs.SetString("Access for " + WRITE_ACCESS, value ? "true" : "false");
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

    private const string CAMERA_ACCESS = "Camera";
    private const string MICROPHONE_ACCESS = "Microphone";
    private const string WRITE_ACCESS = "ExternalStorageWrite";

    public void RequestWriteAccess() {
        if (!MicRequestComplete) {
            if (!HasWriteAccess) {
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            }
            WriteRequestComplete = true;
        } else {
            RequestSettings();
        }
    }

    public void RequestMicAccess() {
        if (!MicRequestComplete) {
            if (!HasMicAccess) {
                Permission.RequestUserPermission(Permission.Microphone);
            }
            MicRequestComplete = true;
        } else {
            RequestSettings();
        }
    }


    public void RequestCameraAccess() {
        if (!CameraRequestComplete) {
            if (!HasCameraAccess) {
                Permission.RequestUserPermission(Permission.Camera);
            }
            CameraRequestComplete = true;
        } else {
            RequestSettings();
        }
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
