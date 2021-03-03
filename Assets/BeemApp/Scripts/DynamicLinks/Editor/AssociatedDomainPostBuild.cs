#if UNITY_IOS
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

using UnityEngine;

public class AssociatedDomainPostBuild : MonoBehaviour {
#if !DEV
    private const string APPLINKS = "applinks:beemrfc.page.link";
#else
    private const string APPLINKS = "applinks:beemrfcdev.page.link";
#endif

    private const string ASSOCIATED_DOMAIN = "BeemAssociatedDomain";

    [PostProcessBuild(998)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path) {
        // Only perform these steps for iOS builds

        string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
        //Default target name. Yours might be different
        string targetName = "Unity-iPhone";
        //Set the entitlements file name to what you want but make sure it has this extension
        string entitlementsFileName = ASSOCIATED_DOMAIN + ".entitlements";

        var entitlements = new ProjectCapabilityManager(projectPath, entitlementsFileName, targetName);
        entitlements.AddAssociatedDomains(new string[] { APPLINKS });
        //Apply
        entitlements.WriteToFile();

    }
}
#endif

