#if UNITY_IOS

using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;

namespace Beem.Xcode {

    /// <summary>
    /// Add framework for FCM 
    /// </summary>
    public class NotificationPostBuild {

        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string path) {
            if (buildTarget == BuildTarget.iOS) {
                LinkLibraries(path);
            }
        }

        static void LinkLibraries(string path) {
            // linked library

            string pbxPath = PBXProject.GetPBXProjectPath(path);

            // Add linked frameworks
            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromString(File.ReadAllText(pbxPath));
            string targetGUID = pbxProject.GetUnityFrameworkTargetGuid();
            pbxProject.AddFrameworkToProject(targetGUID, "UserNotifications.framework", false);
            File.WriteAllText(pbxPath, pbxProject.WriteToString());

        }
    }
}
#endif
