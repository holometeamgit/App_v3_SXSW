using UnityEditor;
using UnityEngine;

public class DevEnvironment : MonoBehaviour
{
    static string IdentifierDev= "com.HoloMe.BeemDev";
    static string IdentifierProd = "com.HoloMe.Beem";
    static string Prod = "PROD";
    static string Dev = "DEV";

    [MenuItem("Environment/Switch To Dev")]
    static void SwitchToDev()
    {
        PlayerSettings.productName = "Beem Dev";
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, Dev);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, Dev);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, Dev);
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, IdentifierDev);
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, IdentifierDev);
        EditorUserBuildSettings.development = true;
    }

    [MenuItem("Environment/Switch To Prod")]
    static void SwitchToProd()
    {
        PlayerSettings.productName = "Beem";
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, Prod);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, Prod);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, Prod);
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, IdentifierProd);
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, IdentifierProd);
        EditorUserBuildSettings.development = false;
    }

#if UNITY_CLOUD_BUILD
    public static void SetAsCloudStaging()
    {
        SwitchToDev();
    }

    public static void SetAsCloudLive()
    {
        SwitchToProd();
    }
#endif

}
