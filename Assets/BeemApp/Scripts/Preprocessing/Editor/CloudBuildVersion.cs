
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

/// <summary>
/// Preprocessing for Build Version
/// </summary>
public class CloudBuildVersion : IPreprocessBuild {

    private static string _versionNumber;
    private static string _buildNumber;

    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildTarget target, string path) {
        _versionNumber = Environment.GetEnvironmentVariable("VERSION_NUMBER");

        _buildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER");

        Debug.Log("Preprocessing...");
        if (!string.IsNullOrEmpty(_versionNumber)) {
            PlayerSettings.bundleVersion = _versionNumber;
        }

        Debug.Log("_versionNumber = " + _versionNumber);

        if (string.IsNullOrEmpty(_buildNumber)) {
            if (target == BuildTarget.iOS) {
                PlayerSettings.iOS.buildNumber = _buildNumber;
            } else if (target == BuildTarget.Android) {
                PlayerSettings.Android.bundleVersionCode = int.Parse(_buildNumber);
            }
        }

        Debug.Log("_buildNumber = " + _buildNumber);
    }
}
