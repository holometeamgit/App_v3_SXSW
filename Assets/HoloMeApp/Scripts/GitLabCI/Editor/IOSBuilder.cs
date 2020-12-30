using System;
using UnityEditor;

namespace Beem.GitLabCI
{

    /// <summary>
    /// Build on IOS
    /// </summary>
    public class IOSBuilder
    {
        static string xcodeFileName = Environment.GetEnvironmentVariable("XCODE_FILE_NAME");

        static string iOSBuildType = Environment.GetEnvironmentVariable("IOS_BUILD_TYPE");

        static string iOSManualProvisioningProfileType = Environment.GetEnvironmentVariable("PROVISION_PROFILE_TYPE");

        static string provisionProfileID = Environment.GetEnvironmentVariable("PROVISION_PROFILE_ID");

        static string teamID = Environment.GetEnvironmentVariable("TEAM_ID");

        /// <summary>
        /// Build Project on iOS
        /// </summary>
        public static void Build()
        {
            EditorUserBuildSettings.iOSBuildConfigType = (iOSBuildType)Enum.Parse(typeof(iOSBuildType), iOSBuildType, true);
            PlayerSettings.iOS.appleDeveloperTeamID = teamID;
            PlayerSettings.iOS.iOSManualProvisioningProfileID = provisionProfileID;
            PlayerSettings.iOS.iOSManualProvisioningProfileType = (ProvisioningProfileType)Enum.Parse(typeof(ProvisioningProfileType), iOSManualProvisioningProfileType, true);
            CIBuild.BuildPlatform(BuildTarget.iOS, BuildTargetGroup.iOS, xcodeFileName);
        }
    }
}
