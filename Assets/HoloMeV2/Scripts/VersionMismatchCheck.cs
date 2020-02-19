using System;
using UnityEngine;

public class VersionMismatchCheck : MonoBehaviour
{
    [SerializeField]
    PnlGenericError pnlGenericError;

    [SerializeField]
    S3Handler s3Handler;

    public Action OnVersionPassed;

    string currentVersion;
    string serverVersion;

    public void CompareVersion()
    {
        s3Handler.DownloadVersionText(OnVersionJSONReceived);
    }

    void OnVersionJSONReceived()
    {
        currentVersion = Application.version;
        VersionJsonData versionData = JsonParser.CreateFromJSON<VersionJsonData>(JsonParser.ParseFileName(HelperFunctions.versionFile));
        serverVersion = Application.platform == RuntimePlatform.Android ? versionData.versionAndroid : versionData.versionIOS;

        if (currentVersion != serverVersion && !versionData.allowOldVersions)
        {
            ShowMismatchMessage();
        }
        else
        {
            if (Application.isEditor)
            {
                Debug.Log($"Version up to date! Server version = {serverVersion}, this version {currentVersion}");
            }
            OnVersionPassed?.Invoke();
        }
    }

    void ShowMismatchMessage()
    {
        pnlGenericError.Activate("New Update Available!", $"Please get the latest version to continue to use the app. {Environment.NewLine} V{currentVersion} - V{serverVersion}", "Open", LinkToStoreAndCloseApp);
    }

    void LinkToStoreAndCloseApp()
    {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=com.HoloMe.Showreel");
#elif UNITY_IPHONE
        Application.OpenURL("itms-apps://apps.apple.com/gb/app/holome/id1454364021");
#endif
        Application.Quit();
    }
}
