using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Beem.SSO {

    /// <summary>
    /// Post Build for adding Firebase data for Google
    /// </summary>
    public static class GoogleSignInPostBuild {
        [PostProcessBuild(999)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath) {
            if (buildTarget == BuildTarget.iOS) {
                var projPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
                var proj = new PBXProject();
                proj.ReadFromFile(projPath);
                var targetGuid = proj.GetUnityMainTargetGuid();
                proj.AddFileToBuild(targetGuid, proj.AddFile("Data/Raw/GoogleService-Info.plist", "GoogleService-Info.plist"));
                proj.WriteToFile(projPath);
            }
        }

    }
}