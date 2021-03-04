#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Beem.Firebase.DynamicLink {

    /// <summary>
    /// Add AppleSignInCapability in Xcode
    /// </summary>
    public class AppleSignInPostBuild {

        private const string ENTITLEMENTS = "AppleSignIn";

        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string path) {
            // Only perform these steps for iOS builds

            string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
            //Default target name. Yours might be different
            string targetName = "Unity-iPhone";
            //Set the entitlements file name to what you want but make sure it has this extension
            string entitlementsFileName = ENTITLEMENTS + ".entitlements";

            var entitlements = new ProjectCapabilityManager(projectPath, entitlementsFileName, targetName);
            entitlements.AddSignInWithApple();
            //Apply
            entitlements.WriteToFile();

        }
    }
}

#endif
