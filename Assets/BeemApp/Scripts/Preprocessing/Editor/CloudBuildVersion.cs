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
                string releaseNotes = string.Format("Build Config Name : {0}, \n Scm Branch : {1}, \n Build Type : {2}, \n Scm Commit ID : {3}, \n Start Build Time : {4}", data.cloudBuildTargetName, data.scmBranch, _buildType, data.scmCommitId, data.buildStartTime);
                Environment.SetEnvironmentVariable(FIREBASE_RELEASE_NOTES, releaseNotes);
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


}
