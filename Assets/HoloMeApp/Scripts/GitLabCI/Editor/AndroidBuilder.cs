using System;
using UnityEditor;

namespace Beem.GitLabCI
{

    /// <summary>
    /// Build on Android
    /// </summary>
    public class AndroidBuilder
    {
        static string productName = Environment.GetEnvironmentVariable("PRODUCT_NAME");

        /// <summary>
        /// Build project on Android
        /// </summary>
        public static void Build()
        {
            CIBuild.BuildPlatform(BuildTarget.Android, BuildTargetGroup.Android, productName + ".apk");
        }
    }
}
