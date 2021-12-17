using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
/// <summary>
/// Preprocessing for Build Version and build name
/// </summary>
public class CloudBuildVersion : IPreprocessBuildWithReport {

    private const string CLOUD_BUILD_MANIFEST = "UnityCloudBuildManifest.json";

    private const string BEEM_VERSION = "BEEM_VERSION";
    private const string BEEM_BUILD = "BEEM_BUILD";
    private const string BEEM_DEV = "BEEM_DEV";
    private const string BEEM_LOG = "BEEM_LOG";
    private const string BEEM_AGREE = "YES";

    private TextAsset currentManifest;

    public int callbackOrder => 0;

    private UnityCloudBuildManifestData GetUnityCloudBuildManifest() {

        if (currentManifest == null) {
            currentManifest = (TextAsset)Resources.Load(CLOUD_BUILD_MANIFEST);
        }

        if (currentManifest != null) {
            return JsonUtility.FromJson<UnityCloudBuildManifestData>(currentManifest.text);
        }

        return new UnityCloudBuildManifestData();
    }

    private void GetEnviromentVariables() {
        SetBuildNumber(Environment.GetEnvironmentVariable(BEEM_BUILD));
        SetBuildVersion(Environment.GetEnvironmentVariable(BEEM_VERSION));
        SetDevType(Environment.GetEnvironmentVariable(BEEM_DEV));
        SetLogType(Environment.GetEnvironmentVariable(BEEM_LOG));
    }

    private void SetBuildNumber(string buildNumber) {
        if (!string.IsNullOrEmpty(buildNumber)) {
            PlayerSettings.iOS.buildNumber = buildNumber;
            PlayerSettings.Android.bundleVersionCode = int.Parse(buildNumber);
        } else {
            if (GetUnityCloudBuildManifest() != null && !string.IsNullOrEmpty(GetUnityCloudBuildManifest().buildNumber)) {
                PlayerSettings.iOS.buildNumber = GetUnityCloudBuildManifest().buildNumber;
                PlayerSettings.Android.bundleVersionCode = int.Parse(GetUnityCloudBuildManifest().buildNumber);
            }
        }
    }

    private void SetBuildVersion(string buildVersion) {
        if (!string.IsNullOrEmpty(buildVersion)) {
            PlayerSettings.bundleVersion = buildVersion;
        }
    }

    private void SetDevType(string isDev) {
        if (!string.IsNullOrEmpty(isDev)) {
            if (isDev == BEEM_AGREE) {
                DevEnvironment.SwitchToDev();
            } else {
                DevEnvironment.SwitchToProd();
            }
        }
    }

    private void SetLogType(string isLog) {
        if (!string.IsNullOrEmpty(isLog)) {
            if (isLog == BEEM_AGREE) {
                DevEnvironment.AddLogs();
            } else {
                DevEnvironment.RemoveLogs();
            }
        }
    }

    public void OnPreprocessBuild(BuildReport report) {
        GetEnviromentVariables();
    }

}
