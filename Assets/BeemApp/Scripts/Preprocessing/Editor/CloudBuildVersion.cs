using Beem.Utility.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
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

    private const string API_KEY = "6c9ddff22a920c8e97a8b2449a9b366b";
    private const string SERVER = "https://build-api.cloud.unity3d.com/api/v1";
    private const string ORG_ID = "10170749378847";

    private static TextAsset currentManifest;

    public int callbackOrder => 0;

    private static UnityCloudBuildManifestData GetUnityCloudBuildManifest() {

        if (currentManifest == null) {
            currentManifest = (TextAsset)Resources.Load(CLOUD_BUILD_MANIFEST);
        }

        if (currentManifest != null) {
            return JsonUtility.FromJson<UnityCloudBuildManifestData>(currentManifest.text);
        }

        return new UnityCloudBuildManifestData();
    }

    private static string GetUrl() {
        if (GetUnityCloudBuildManifest() != null) {
            return $"{SERVER}/orgs/{ORG_ID}/projects/{GetUnityCloudBuildManifest().projectId}/buildtargets/{GetUnityCloudBuildManifest().cloudBuildTargetName}/envvars";
        }
        return string.Empty;
    }

    private static void WriteReleaseNotes(FirebaseEnviromentVariables firebaseEnviromentVariables) {
        //if (GetUnityCloudBuildManifest() != null) {
        firebaseEnviromentVariables.FIREBASE_RELEASE_NOTES = Application.dataPath + "/Resources/" + CLOUD_BUILD_MANIFEST;//$"Build Config Name - {GetUnityCloudBuildManifest().cloudBuildTargetName}, \n Scm Branch - {GetUnityCloudBuildManifest().scmBranch}, \n Scm Commit ID - {GetUnityCloudBuildManifest().scmCommitId}";
        //}
    }

    [MenuItem("UCB/GetEnviromentVariables")]
    public static void GetEnviromentVariables() {
        Debug.LogError("GetEnviromentVariables");

        Hashtable hashtable = Environment.GetEnvironmentVariables() as Hashtable;

        foreach (DictionaryEntry entry in hashtable) {
            Debug.LogError(entry.Key + " " + entry.Value);
        }

        GetRequest request = new GetRequest(GetUrl(), "Basic " + API_KEY);
        Debug.LogError("GetRequest");
        request.Send(ViewEnviromentVariables);
        Debug.LogError("Send");
    }

    private static void ViewEnviromentVariables(string body) {
        Debug.LogError("ViewEnviromentVariables " + body);
        FirebaseEnviromentVariables firebaseEnviromentVariables = JsonUtility.FromJson<FirebaseEnviromentVariables>(body);
        SetBuildNumber(firebaseEnviromentVariables.BEEM_BUILD);
        SetBuildVersion(firebaseEnviromentVariables.BEEM_VERSION);
        SetBuildType(firebaseEnviromentVariables.BEEM_BUILD_TYPE);
        WriteReleaseNotes(firebaseEnviromentVariables);
        SetEnviromentVariables(firebaseEnviromentVariables);
    }

    private static void SetBuildNumber(string buildNumber) {
        Debug.LogError("SetBuildNumber " + buildNumber);
        if (!string.IsNullOrEmpty(buildNumber)) {
            PlayerSettings.iOS.buildNumber = buildNumber;
            PlayerSettings.Android.bundleVersionCode = int.Parse(buildNumber);
        } else {
            if (GetUnityCloudBuildManifest() != null) {
                PlayerSettings.iOS.buildNumber = GetUnityCloudBuildManifest().buildNumber;
                PlayerSettings.Android.bundleVersionCode = int.Parse(GetUnityCloudBuildManifest().buildNumber);
            }
        }
    }

    private static void SetBuildVersion(string buildVersion) {
        Debug.LogError("SetBuildVersion " + buildVersion);
        if (!string.IsNullOrEmpty(buildVersion)) {
            PlayerSettings.bundleVersion = buildVersion;
        }
    }

    private static void SetBuildType(string buildType) {
        Debug.LogError("SetBuildType " + buildType);
        if (!string.IsNullOrEmpty(buildType)) {
            PlayerSettings.productName = buildType == DEV ? APPLICATION_NAME_DEV : APPLICATION_NAME;
            EditorUserBuildSettings.development = buildType == DEV;
            SwitchDefine(BuildTargetGroup.iOS, buildType);
            SwitchDefine(BuildTargetGroup.Android, buildType);
        }
    }

    private static void SetEnviromentVariables(FirebaseEnviromentVariables firebaseEnviromentVariables) {
        Debug.LogError("SetEnviromentVariables " + firebaseEnviromentVariables.FIREBASE_RELEASE_NOTES);
        PutRequest<FirebaseEnviromentVariables> request = new PutRequest<FirebaseEnviromentVariables>(GetUrl(), firebaseEnviromentVariables, "Basic " + API_KEY);
        request.Send();
    }

    private static void SwitchDefine(BuildTargetGroup targetGroup, string buildType) {
        Debug.LogError("SwitchDefine " + targetGroup + "," + buildType);
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
        Debug.LogError("OnPreprocessBuild");
        GetEnviromentVariables();
    }

}
