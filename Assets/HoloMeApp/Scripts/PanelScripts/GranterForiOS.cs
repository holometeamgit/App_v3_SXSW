using UnityEngine;

public class GranterForiOS : IPermissionGranter {

    public bool HasMicAccess => Application.HasUserAuthorization(UserAuthorization.Microphone);
    public bool HasWriteAccess => true;

    public bool HasCameraAccess => Application.HasUserAuthorization(UserAuthorization.WebCam);

    public bool MicRequestComplete {
        get {
            return PlayerPrefs.GetString("Access for " + MICROPHONE_ACCESS, "false") == "true";
        }
        private set {
            PlayerPrefs.SetString("Access for " + MICROPHONE_ACCESS, value ? "true" : "false");
        }
    }
    public bool WriteRequestComplete { get; private set; }
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

    public void RequestCameraAccess() {
        if (CameraRequestComplete) {
            if (!HasCameraAccess) {
                Application.RequestUserAuthorization(UserAuthorization.WebCam);
            }
            CameraRequestComplete = true;
        } else {
            RequestSettings();
        }
    }

    public void RequestMicAccess() {
        if (!MicRequestComplete) {
            if (!HasMicAccess) {
                Application.RequestUserAuthorization(UserAuthorization.Microphone);
            }
            MicRequestComplete = true;
        } else {
            RequestSettings();
        }
    }

    public void RequestWriteAccess() {
        WriteRequestComplete = true;
    }

    public void RequestSettings() {
#if UNITY_IOS && !UNITY_EDITOR
        string url = iOSSettingsOpenerBindings.GetSettingsURL();
        Application.OpenURL(url);
#endif
    }

}
