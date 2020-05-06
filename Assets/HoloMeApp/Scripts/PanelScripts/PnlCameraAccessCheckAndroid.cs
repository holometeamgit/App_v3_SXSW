using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PnlCameraAccessCheckAndroid : MonoBehaviour, IPermissionGranter
{
    bool cameraPermissionGranted;

    [SerializeField]
    Button btnRequestPermission;

    public bool MicRequestComplete { get; private set; }
    public bool WriteRequestComplete { get; private set; }

    public bool MicAccessAvailable => Permission.HasUserAuthorizedPermission(Permission.Microphone);
    public bool WriteAccessAvailable => Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite);
    public bool HasCameraAccess => Permission.HasUserAuthorizedPermission(Permission.Camera);

    void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "Main")
            return;
        if (Application.platform != RuntimePlatform.Android)
            return;

        StartCoroutine(VerifyPermissionLive());

        if (btnRequestPermission == null)
            btnRequestPermission = GameObject.Find("btnSettings").GetComponent<Button>();
        btnRequestPermission.onClick.AddListener(RequestCameraAccess);
    }

    public void RequestWriteAccess()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
        WriteRequestComplete = true;
    }

    public void RequestMicAccess()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
        MicRequestComplete = true;
    }

    IEnumerator VerifyPermissionLive()
    {
        while (!cameraPermissionGranted)
        {
            if (Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                OnCameraAccessVerified("true");
            }
            yield return null;
        }
    }

    void OnCameraAccessVerified(string permissionWasGranted)
    {
        if (permissionWasGranted == "true" && !cameraPermissionGranted)
        {
            print("Load SCENE CALLED");
            //gameObject.SetActive(false);
            cameraPermissionGranted = true;
            SceneManager.LoadScene(1);
            print("POST LOAD");
        }
    }

    public void RequestCameraAccess()
    {
        AndroidRuntimePermissions.Permission[] result = AndroidRuntimePermissions.RequestPermissions("android.permission.CAMERA");
        //AndroidRuntimePermissions.Permission[] result = AndroidRuntimePermissions.RequestPermissions("android.permission.WRITE_EXTERNAL_STORAGE", "android.permission.CAMERA", "android.permission.RECORD_AUDIO");
    }

}
