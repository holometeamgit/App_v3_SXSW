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

    private const string APPLICATION_NAME_DEV = "Beem Dev";
    private const string APPLICATION_NAME = "Beem";

    private const string PROD = "PROD";
    private const string DEV = "DEV";

    private string[] iOSDevDefines = new string[] {
        "UNITY_XR_ARKIT_LOADER_ENABLED",
        "CT_BWF",
        DEV
    };

    private string[] iOSProdDefines = new string[] {
        "UNITY_XR_ARKIT_LOADER_ENABLED",
        "CT_BWF",
        PROD
    };

    private string[] AndroidDevDefines = new string[] {
        "CT_BWF",
        DEV
    };

    private string[] AndroidProdDefines = new string[] {
        "CT_BWF",
        PROD
    };

    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildTarget target, string path) {
        _versionNumber = Environment.GetEnvironmentVariable(VERSION);

        _buildNumber = Environment.GetEnvironmentVariable(BUILD_NUMBER);

        _buildType = Environment.GetEnvironmentVariable(BUILD_TYPE);

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, EditorUserBuildSettings.development ? DEV : PROD);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, EditorUserBuildSettings.development ? DEV : PROD);

        if (!string.IsNullOrEmpty(_versionNumber)) {

            List<int> versions = GetVersions(_versionNumber);

            List<int> previousVersions = GetVersions(PlayerSettings.bundleVersion);

            for (int i = 0; i < Mathf.Min(versions.Count, previousVersions.Count); i++) {
                if (versions[i] > previousVersions[i]) {
                    PlayerSettings.bundleVersion = _versionNumber;
                    break;
                }
            }
        }

        if (!string.IsNullOrEmpty(_buildNumber)) {
            if (int.Parse(_buildNumber) > int.Parse(PlayerSettings.iOS.buildNumber)) {
                PlayerSettings.iOS.buildNumber = _buildNumber;
            }
            if (int.Parse(_buildNumber) > PlayerSettings.Android.bundleVersionCode) {
                PlayerSettings.Android.bundleVersionCode = int.Parse(_buildNumber);
            }
        }

        if (!string.IsNullOrEmpty(_buildType)) {
            PlayerSettings.productName = _buildType == DEV ? APPLICATION_NAME_DEV : APPLICATION_NAME;
            EditorUserBuildSettings.development = _buildType == DEV;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, _buildType == DEV ? iOSDevDefines : iOSProdDefines);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, _buildType == DEV ? AndroidDevDefines : AndroidProdDefines);
        }
    }

    private List<int> GetVersions(string version) {

        List<int> temp = new List<int>();

        version = version.Trim();
        string[] lines = version.Split('.');

        for (int i = 0; i < lines.Length; i++) {
            temp.Add(int.Parse(lines[i]));
        }

        return temp;
    }

}
