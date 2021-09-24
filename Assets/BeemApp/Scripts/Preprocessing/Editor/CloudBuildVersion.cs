using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
/// <summary>
/// Preprocessing for Build Version and build name
/// </summary>
public class CloudBuildVersionpublic : IPreprocessBuild {

    private static string _versionNumber;
    private static string _buildNumber;
    private static string _buildType;
    private static string _releaseNotes;

    private const string VERSION = "BEEM_VERSION";
    private const string BUILD_NUMBER = "BEEM_BUILD";
    private const string BUILD_TYPE = "BEEM_BUILD_TYPE";
    private const string FIREBASE_RELEASE_NOTES = "FIREBASE_RELEASE_NOTES";
    private const string CLOUD_BUILD_MANIFEST = "UnityCloudBuildManifest.json";

    private const string APPLICATION_NAME_DEV = "Beem Dev";
    private const string APPLICATION_NAME = "Beem";

    private const string PROD = "PROD";
    private const string DEV = "DEV";

    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildTarget target, string path) {
        _versionNumber = Environment.GetEnvironmentVariable(VERSION);

        _buildNumber = Environment.GetEnvironmentVariable(BUILD_NUMBER);

        _buildType = Environment.GetEnvironmentVariable(BUILD_TYPE);

        _releaseNotes = Environment.GetEnvironmentVariable(FIREBASE_RELEASE_NOTES);

        if (!string.IsNullOrEmpty(_versionNumber)) {
            PlayerSettings.bundleVersion = _versionNumber;
        }

        if (!string.IsNullOrEmpty(_buildNumber)) {
            PlayerSettings.iOS.buildNumber = _buildNumber;
            PlayerSettings.Android.bundleVersionCode = int.Parse(_buildNumber);
        } else {
            UnityCloudBuildManifestData data = GetData();
            if (data != null) {
                PlayerSettings.iOS.buildNumber = data.buildNumber;
                PlayerSettings.Android.bundleVersionCode = int.Parse(data.buildNumber);
                _releaseNotes = string.Format("Build Config Name : {0}, \n Scm Branch : {1}, \n Build Type : {2}, \n Scm Commit ID : {3}, \n Start Build Time : {4}", data.cloudBuildTargetName, data.scmBranch, _buildType, data.scmCommitId, data.buildStartTime);
                HelperFunctions.DevLog(_releaseNotes);
                Environment.SetEnvironmentVariable(FIREBASE_RELEASE_NOTES, _releaseNotes);
            }
        }

        if (!string.IsNullOrEmpty(_buildType)) {
            PlayerSettings.productName = _buildType == DEV ? APPLICATION_NAME_DEV : APPLICATION_NAME;
            EditorUserBuildSettings.development = _buildType == DEV;
            SwitchDefine(BuildTargetGroup.iOS, _buildType);
            SwitchDefine(BuildTargetGroup.Android, _buildType);
            HelperFunctions.DevLog(PlayerSettings.productName);
            HelperFunctions.DevLog(EditorUserBuildSettings.development.ToString());
            HelperFunctions.DevLog(PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android));
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

    private UnityCloudBuildManifestData GetData() {
        var manifest = (TextAsset)Resources.Load(CLOUD_BUILD_MANIFEST);
        if (manifest != null) {
            return JsonUtility.FromJson<UnityCloudBuildManifestData>(manifest.text);
        }
        return null;
    }


    [Serializable]
    public class UnityCloudBuildManifestData {
        public string scmCommitId;
        public string scmBranch;
        public string buildNumber;
        public string buildStartTime;
        public string projectId;
        public string bundleId;
        public string unityVersion;
        public string xcodeVersion;
        public string cloudBuildTargetName;
    }

    [Serializable]
    public class BuildStatus {
        public int build;
        public string buildtargetid;
        public string buildTargetName;
        public string buildStatus;
        public string created;
        public string finished;
        public string commitId;
        public string message;
        public string scmBranch;

        /*
    
        "artifacts": [
            {
                "key": "primary",
                "name": ".APK file",
                "primary": true,
                "show_download": true,
                "files": [
                    {
            "filename": "FirebaseAPKDev Test Manifest.apk",
                        "size": 177081609,
                        "resumable": false,
                        "md5sum": "vf8uyvyLuPaYC+T8Yy07Sw==",
                        "href": "https://storage.googleapis.com/unitycloud-build-user-svc-live-build/10170749378847%2Fba3333ea-4845-4d5b-a4c1-896277975428%2Ffirebaseapkdev-test-manifest-10%2FFirebaseAPKDev%20Test%20Manifest.apk?GoogleAccessId=signurl-sa%40unity-cs-cloudbuild-prd.iam.gserviceaccount.com&Expires=1632486721&Signature=W%2BN4lOcwU%2FU2kiTwTvDjCG6L5%2FXQdtPUkLk5cEX3vveiP0U54tLsc%2Fn2ZPBE8%2FiNVyWlZP%2FqaeR1r4K98cDLoBY05YHjS0C6UQtipXw9zKMIf0XpIPYFOW7Ll%2FiPGbuzLxZY5fVZOcFnmawKMFcf%2FK2OCEqDLuhHJN5RMDuDVZLlulmf74LosATQwi62lF1pBwLayhxq212gqs3dMYlSU8yM%2FMgMXJdTwWlZCJNNtcZwT6z0btO8sVqnWapzL68UIRtEC4G%2BwE%2BjmnW0WNPo5ruokxo1FEyDxQq9vhQKi4MSQXI3v4Y1vJ0D3EoSObs%2BfCzClqNaIDSkcDLzuvfrOQ%3D%3D&response-content-type=application%2Foctet-stream&response-content-disposition=attachment%3B%20filename%3Dholome_beem-app_v3_sxsw-firebaseapkdev-test-manifest-10.apk"
                    }
                ]
            },
        ]
        */

    }



}
