using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PnlCameraAccessCheckAndroid : MonoBehaviour, IPermissionGranter {
    bool cameraPermissionGranted;

    [SerializeField]
    Button btnRequestPermission;

    public bool HasCameraAccess => Permission.HasUserAuthorizedPermission(Permission.Camera);
    public bool MicAccessAvailable => Permission.HasUserAuthorizedPermission(Permission.Microphone);
    public bool WriteAccessAvailable => Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite);

    public bool MicRequestComplete { get; private set; }
    public bool WriteRequestComplete { get; private set; }
    public bool CameraRequestComplete { get; private set; }

    void OnEnable() {
        if (Application.platform != RuntimePlatform.Android)
            return;

        StartCoroutine(VerifyPermissionLive());
    }

    public void RequestWriteAccess() {
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite)) {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
        WriteRequestComplete = true;
    }

    public void RequestMicAccess() {
        if (!MicRequestComplete) {
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone)) {
                Permission.RequestUserPermission(Permission.Microphone);
            }
            MicRequestComplete = true;
        } else {
            NativeRequestSettings();
        }
    }


    public void RequestCameraAccess() {
        if (!CameraRequestComplete) {
            AndroidRuntimePermissions.Permission[] result = AndroidRuntimePermissions.RequestPermissions("android.permission.CAMERA");
        } else {
            NativeRequestSettings();
        }
    }

    private void NativeRequestSettings() {
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

    IEnumerator VerifyPermissionLive() {
        CameraRequestComplete = true;
        while (!cameraPermissionGranted) {
            if (Permission.HasUserAuthorizedPermission(Permission.Camera)) {
                OnCameraAccessVerified("true");
            }
            yield return null;
        }
    }

    void OnCameraAccessVerified(string permissionWasGranted) {
        if (permissionWasGranted == "true" && !cameraPermissionGranted) {
            cameraPermissionGranted = true;
        }
    }

    public void RequestSettings()
    {
        NativeRequestSettings();
    }
}
