using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace Beem.GitLabCI
{
    /// <summary>
    /// Application Build
    /// </summary>
    public class CIBuild
    {
        static string appBundle = Environment.GetEnvironmentVariable("APP_BUNDLE");

        static string productName = Environment.GetEnvironmentVariable("PRODUCT_NAME");

        /// <summary>
        /// Common Build
        /// </summary>
        /// <param name="target"></param>
        /// <param name="targetGroup"></param>
        /// <param name="fileName"></param>
        public static void BuildPlatform(BuildTarget target, BuildTargetGroup targetGroup, string fileName)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, target);

            PlayerSettings.applicationIdentifier = appBundle;
            PlayerSettings.productName = productName;

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = GetScenes();
            buildPlayerOptions.locationPathName = fileName;
            buildPlayerOptions.target = target;
            buildPlayerOptions.options = BuildOptions.None;

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        static string[] GetScenes()
        {
            var projectScenes = EditorBuildSettings.scenes;
            List<string> scenesToBuild = new List<string>();
            for (int i = 0; i < projectScenes.Length; i++)
            {
                if (projectScenes[i].enabled)
                {
                    scenesToBuild.Add(projectScenes[i].path);
                }
            }
            return scenesToBuild.ToArray();
        }
    }
}
