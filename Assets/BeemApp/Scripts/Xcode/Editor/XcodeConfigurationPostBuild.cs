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

        private const string DOMAIN = "https://join.beem.me";
        private const string DOMAIN_ROOM = "https://join.beem.me/room";

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

            // custom domain modes
            PlistElementArray customDomain = rootDict.CreateArray("FirebaseDynamicLinksCustomDomains");
            customDomain.AddString(DOMAIN);
            customDomain.AddString(DOMAIN_ROOM);

            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());

        }
    }
}
#endif