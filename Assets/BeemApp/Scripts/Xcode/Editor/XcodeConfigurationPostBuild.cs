#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Beem.Xcode {

    /// <summary>
    /// Add Plist Configuration
    /// </summary>
    public class XcodeConfigurationPostBuild {
        [PostProcessBuild]
        public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject) {

            // Get plist
            string plistPath = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            // Get root
            PlistElementDict rootDict = plist.root;

            // background location useage key (new in iOS 8)
            rootDict.SetString("Privacy - Local Network Usage Description", "Beem uses this to find and connect to devices to cast to your TV");

            // background modes
            PlistElementArray bgModes = rootDict.CreateArray("Bonjour services");
            bgModes.AddString("_beem._tcp");

            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());

        }
    }
}
#endif