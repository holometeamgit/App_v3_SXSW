#if UNITY_IOS || UNITY_TVOS
#define UNITY_XCODE_EXTENSIONS_AVAILABLE
#endif

using AppleAuth.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_XCODE_EXTENSIONS_AVAILABLE
using UnityEditor.iOS.Xcode;
#endif

namespace Beem.Xcode {

    /// <summary>
    /// Add Capability to Xcode
    /// </summary>
    public class CapabilityPostBuild {

        private static string[] applinks = new string[] {
            "applinks:ar.beem.me",
            //"applinks:join.beem.me",
            //"applinks:beem.onelink.me"
        };

        private const int CallOrder = 2;

        [PostProcessBuild(CallOrder)]
        public static void OnPostProcessBuild(BuildTarget target, string path) {
            if (target == BuildTarget.iOS || target == BuildTarget.tvOS) {
#if UNITY_XCODE_EXTENSIONS_AVAILABLE
                var projectPath = PBXProject.GetPBXProjectPath(path);
#if UNITY_2019_3_OR_NEWER
                var project = new PBXProject();
                project.ReadFromString(System.IO.File.ReadAllText(projectPath));
                var manager = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", null, project.GetUnityMainTargetGuid());
                manager.AddSignInWithAppleWithCompatibility(project.GetUnityFrameworkTargetGuid());
#if DEV
                manager.AddPushNotifications(true);
#else
                manager.AddPushNotifications(false);
#endif
                manager.AddAssociatedDomains(applinks);
                manager.WriteToFile();
#else
                        var manager = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", PBXProject.GetUnityTargetName());
                        manager.AddSignInWithAppleWithCompatibility();
                        manager.WriteToFile();
#endif
#endif
            } else if (target == BuildTarget.StandaloneOSX) {
                AppleAuthMacosPostprocessorHelper.FixManagerBundleIdentifier(target, path);
            }
        }

    }
}
