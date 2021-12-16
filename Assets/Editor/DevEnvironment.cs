using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DevEnvironment : MonoBehaviour {

    private const string APPLICATION_NAME_DEV = "Beem Dev";
    private const string APPLICATION_NAME = "Beem";

    private const string DEV = "DEV";
    private const string LOG = "LOG";

    [MenuItem("Environment/Switch To Dev")]
    public static void SwitchToDev() {
        PlayerSettings.productName = APPLICATION_NAME_DEV;

        AddDefine(BuildTargetGroup.iOS, DEV);
        AddDefine(BuildTargetGroup.Android, DEV);

        EditorUserBuildSettings.development = true;
    }

    [MenuItem("Environment/Switch To Prod")]
    public static void SwitchToProd() {
        PlayerSettings.productName = APPLICATION_NAME;

        RemoveDefine(BuildTargetGroup.iOS, DEV);
        RemoveDefine(BuildTargetGroup.Android, DEV);

        EditorUserBuildSettings.development = false;
    }

    [MenuItem("Environment/Add Logs")]
    public static void AddLogs() {
        AddDefine(BuildTargetGroup.iOS, LOG);
        AddDefine(BuildTargetGroup.Android, LOG);
    }

    [MenuItem("Environment/Remove Logs")]
    public static void RemoveLogs() {
        RemoveDefine(BuildTargetGroup.iOS, LOG);
        RemoveDefine(BuildTargetGroup.Android, LOG);
    }

    private static void AddDefine(BuildTargetGroup targetGroup, string buildType) {
        string[] currentDefines;
        PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out currentDefines);

        if (!currentDefines.Contains(buildType)) {
            List<string> nextDefines = currentDefines.ToList();
            nextDefines.Add(buildType);
            currentDefines = nextDefines.ToArray();
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, currentDefines);
    }

    private static void RemoveDefine(BuildTargetGroup targetGroup, string buildType) {
        string[] currentDefines;
        PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out currentDefines);

        if (currentDefines.Contains(buildType)) {
            List<string> nextDefines = currentDefines.ToList();
            nextDefines.Remove(buildType);
            currentDefines = nextDefines.ToArray();
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, currentDefines);
    }

#if UNITY_CLOUD_BUILD
    public static void BuildDev()
    {
        SwitchToDev();
    }

    public static void BuildProd()
    {
        SwitchToProd();
    }
#endif

}
