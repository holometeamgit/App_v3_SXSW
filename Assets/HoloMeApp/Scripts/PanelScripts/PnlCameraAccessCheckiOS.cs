using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PnlCameraAccessCheckiOS : MonoBehaviour, IPermissionGranter
{
    bool cameraPermissionGranted;

    [SerializeField]
    Button btnRequestPermission;

    public bool MicRequestComplete { get; private set; }
    public bool WriteRequestComplete { get; private set; }

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

    void OnEnable()
    {
        iOSCameraPermission.VerifyPermission(gameObject.name, "OnCameraAccessVerified");

        if (SceneManager.GetActiveScene().name == "Main")
            return;
        if (Application.platform != RuntimePlatform.IPhonePlayer)
            return;

        StartCoroutine(VerifyPermissionLive());

        if (btnRequestPermission == null)
            btnRequestPermission = GameObject.Find("btnSettings").GetComponent<Button>();
        btnRequestPermission.onClick.AddListener(RequestCameraAccess);
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
            //gameObject.SetActive(false);
            cameraPermissionGranted = true;

            //if (!MicAccessAvailable)
            //{
            //    StartCoroutine(AsyncMicAccess());
            //}
            //else
            //{
            //    SceneManager.LoadScene(1);
            //}
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
        Application.RequestUserAuthorization(UserAuthorization.Microphone);
        MicRequestComplete = true;
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
