using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DevEnvironment : MonoBehaviour
{
    [MenuItem("Dev/Switch To Staging")]
    static void SwitchToStaging()
    {
        PlayerSettings.productName = "HoloMe Staging";
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "STAGING");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "STAGING");
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.HoloMe.ShowreelStaging");
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.HoloMe.ShowreelStaging");
        EditorUserBuildSettings.development = true;
    }

    [MenuItem("Dev/Switch To Live")]
    static void SwitchToLive()
    {
        PlayerSettings.productName = "HoloMe";
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "LIVE");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "LIVE");
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.HoloMe.Showreel");
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.HoloMe.Showreel");
        EditorUserBuildSettings.development = false;
    }

#if UNITY_CLOUD_BUILD
    public static void SetAsCloudStaging()
    {
        SwitchToStaging();
    }

    public static void SetAsCloudLive()
    {
        SwitchToLive();
    }
#endif

}
