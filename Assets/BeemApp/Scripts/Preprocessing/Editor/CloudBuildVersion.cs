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

    private const string VERSION = "BEEM_VERSION";
    private const string BUILD = "BEEM_BUILD";

    private const string PROD = "PROD";
    private const string DEV = "DEV";

    private const string PROD_NAME = "Beem";
    private const string DEV_NAME = "Beem Dev";

    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildTarget target, string path) {
        _versionNumber = Environment.GetEnvironmentVariable(VERSION);

        _buildNumber = Environment.GetEnvironmentVariable(BUILD);

        if (EditorUserBuildSettings.development) {
            PlayerSettings.productName = DEV_NAME;

            SwitchDefine(BuildTargetGroup.iOS, PROD, DEV);
            SwitchDefine(BuildTargetGroup.Android, PROD, DEV);
        } else {
            PlayerSettings.productName = PROD_NAME;

            SwitchDefine(BuildTargetGroup.iOS, DEV, PROD);
            SwitchDefine(BuildTargetGroup.Android, DEV, PROD);
        }

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

    private void SwitchDefine(BuildTargetGroup targetGroup, string firstDefine, string secondDefine) {
        string[] currentDefines;
        PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out currentDefines);
        if (currentDefines.Contains(firstDefine)) {
            for (int i = 0; i < currentDefines.Length; i++) {
                if (currentDefines[i] == firstDefine) {
                    currentDefines[i] = secondDefine;
                    break;
                }
            }
        } else {
            int lenght = currentDefines.Length;
            currentDefines = new string[lenght + 1];
            currentDefines[lenght] = secondDefine;
        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, currentDefines);
    }
}
