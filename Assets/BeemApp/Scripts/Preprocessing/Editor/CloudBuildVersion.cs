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

    private const string APPLICATION_NAME_DEV = "Beem Dev";
    private const string APPLICATION_NAME = "Beem";

    private const string PROD = "PROD";
    private const string DEV = "DEV";

    private const string BEEM_VERSION = "BEEM_VERSION";
    private const string BEEM_BUILD = "BEEM_BUILD";
    private const string BEEM_BUILD_TYPE = "BEEM_BUILD_TYPE";

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
        SetBuildType(Environment.GetEnvironmentVariable(BEEM_BUILD_TYPE));
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

    private void SetBuildType(string buildType) {
        if (!string.IsNullOrEmpty(buildType)) {
            PlayerSettings.productName = buildType == DEV ? APPLICATION_NAME_DEV : APPLICATION_NAME;
            EditorUserBuildSettings.development = buildType == DEV;
            SwitchDefine(BuildTargetGroup.iOS, buildType);
            SwitchDefine(BuildTargetGroup.Android, buildType);
        }
    }

    private void SwitchDefine(BuildTargetGroup targetGroup, string buildType) {
        string[] currentDefines;
        PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out currentDefines);
        if (currentDefines.Contains(PROD)) {
            for (int i = 0; i < currentDefines.Length; i++) {
                if (currentDefines[i] == PROD) {
                    currentDefines[i] = buildType;
                }
            }
        } else if (currentDefines.Contains(DEV)) {
            for (int i = 0; i < currentDefines.Length; i++) {
                if (currentDefines[i] == DEV) {
                    currentDefines[i] = buildType;
                }
            }
        } else {
            int lenght = currentDefines.Length;
            currentDefines = new string[lenght + 1];
            currentDefines[lenght] = buildType;
        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, currentDefines);
    }

    public void OnPreprocessBuild(BuildReport report) {
        GetEnviromentVariables();
    }

}
