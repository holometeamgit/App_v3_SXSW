#if UNITY_IOS

using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using UnityEngine;

namespace Beem.CleverTap {

    /// <summary>
    /// Add framework for CleverTap 
    /// </summary>
    public class CleverTapPostBuild {
        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string path) {
            if (buildTarget == BuildTarget.iOS) {
                LinkLibraries(path);
            }
        }

        static void LinkLibraries(string path) {
            // linked library
            string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
            PBXProject proj = new PBXProject();
            proj.ReadFromFile(projPath);

            // embedded frameworks
            string target = proj.GetUnityMainTargetGuid();
            const string defaultLocationInProj = "Frameworks/CleverTapUnity/iOS";
            const string CleverTapSDKName = "CleverTapSDK.framework";
            const string SDWebImageName = "SDWebImage.framework";

            string fw1 = Path.Combine(defaultLocationInProj, CleverTapSDKName);
            string fw2 = Path.Combine(defaultLocationInProj, SDWebImageName);
            string fileGuid = proj.AddFile(fw1, fw1, PBXSourceTree.Source);
            proj.AddFileToEmbedFrameworks(target, fileGuid);
            fileGuid = proj.AddFile(fw2, fw2, PBXSourceTree.Source);
            proj.AddFileToEmbedFrameworks(target, fileGuid);

            //Shell script
            var text = Resources.Load<TextAsset>("CleverTap");
            proj.AddShellScriptBuildPhase(target, "Build Framework", "/bin/sh", text.text);

            // done, write to the project file
            File.WriteAllText(projPath, proj.WriteToString());
        }
    }
}
#endif
