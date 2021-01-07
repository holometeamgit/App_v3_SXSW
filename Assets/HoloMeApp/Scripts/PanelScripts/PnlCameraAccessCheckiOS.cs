using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PnlCameraAccessCheckiOS : MonoBehaviour, IPermissionGranter
{
    bool cameraPermissionGranted;

    [SerializeField]
    Button btnRequestPermission;

    public bool MicAccessAvailable => Application.HasUserAuthorization(UserAuthorization.Microphone);
    public bool WriteAccessAvailable => true;

    public bool HasCameraAccess
    {
        get
        {
            iOSCameraPermission.VerifyPermission(gameObject.name, "OnCameraAccessVerified");
            return cameraPermissionGranted;
        }
    }

    public bool MicRequestComplete { get; private set; }
    public bool WriteRequestComplete { get; private set; }
    public bool CameraRequestComplete { get; private set; }

    void OnEnable()
    {
        iOSCameraPermission.VerifyPermission(gameObject.name, "OnCameraAccessVerified");
        CameraRequestComplete = true;
        if (Application.platform != RuntimePlatform.IPhonePlayer)
            return;

        StartCoroutine(VerifyPermissionLive());
    }

    IEnumerator VerifyPermissionLive()
    {
        while (!cameraPermissionGranted)
        {
            iOSCameraPermission.VerifyPermission(gameObject.name, "OnCameraAccessVerified");
            yield return null;
        }
    }

    void OnCameraAccessVerified(string permissionWasGranted)
    {
        if (permissionWasGranted == "true" && !cameraPermissionGranted)
        {
            cameraPermissionGranted = true;
        }
    }

    public void RequestCameraAccess()
    {
#if UNITY_IOS && !UNITY_EDITOR
        string url = iOSSettingsOpenerBindings.GetSettingsURL();
        Application.OpenURL(url);
#endif
    }

    public void RequestMicAccess()
    {
        if (!MicRequestComplete) {
            Application.RequestUserAuthorization(UserAuthorization.Microphone);
            MicRequestComplete = true;
        } else {
#if UNITY_IOS && !UNITY_EDITOR
        string url = iOSSettingsOpenerBindings.GetSettingsURL();
        Application.OpenURL(url);
#endif
        }
    }

    IEnumerator AsyncMicAccess()
    {
        var asyncOp = Application.RequestUserAuthorization(UserAuthorization.Microphone);
        asyncOp.completed += x => SceneManager.LoadScene(1);

        while (!asyncOp.isDone)
        {
            yield return null;
        }
    }

    public void RequestWriteAccess()
    {
        WriteRequestComplete = true;
    }

}
