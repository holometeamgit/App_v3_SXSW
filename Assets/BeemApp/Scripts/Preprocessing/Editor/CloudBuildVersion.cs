using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
/// <summary>
/// Preprocessing for Build Version
/// </summary>
public class CloudBuildVersion {

#if UNITY_CLOUD_BUILD
    public static void PreExport(UnityEngine.CloudBuild.BuildManifestObject manifest) {
        string buildNumber = manifest.GetValue("buildNumber", "0");

        Debug.LogWarning("Setting build number to " + buildNumber);

        if (string.IsNullOrEmpty(buildNumber)) {
            PlayerSettings.iOS.buildNumber = buildNumber;
            PlayerSettings.Android.bundleVersionCode = int.Parse(buildNumber);
        }

        Debug.Log("PlayerSettings.iOS.buildNumber = " + PlayerSettings.iOS.buildNumber);
        Debug.Log("PlayerSettings.Android.bundleVersionCode = " + PlayerSettings.Android.bundleVersionCode);
    }
#endif
}
