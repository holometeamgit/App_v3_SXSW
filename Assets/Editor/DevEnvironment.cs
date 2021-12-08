using System.Linq;
using UnityEditor;
using UnityEngine;

public class DevEnvironment : MonoBehaviour {
    static string PROD = "PROD";
    static string DEV = "DEV";

    [MenuItem("Environment/Switch To Dev")]
    static void SwitchToDev() {
        PlayerSettings.productName = "Beem Dev";

        SwitchDefine(BuildTargetGroup.iOS, DEV);
        SwitchDefine(BuildTargetGroup.Android, DEV);

        EditorUserBuildSettings.development = true;
    }

    [MenuItem("Environment/Switch To Prod")]
    static void SwitchToProd() {
        PlayerSettings.productName = "Beem";

        SwitchDefine(BuildTargetGroup.iOS, PROD);
        SwitchDefine(BuildTargetGroup.Android, PROD);

        EditorUserBuildSettings.development = false;
    }

    private static void SwitchDefine(BuildTargetGroup targetGroup, string buildType) {
        string[] currentDefines;
        PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out currentDefines);
        if (currentDefines.Contains(PROD)) {
            for (int i = 0; i < currentDefines.Length; i++) {
                if (currentDefines[i] == PROD) {
                    currentDefines[i] = buildType;
                }
            }
        } else if (currentDefines.Contains(DEV)) {
            for (int i = 0; i < currentDefines.Length; i++) {
                if (currentDefines[i] == DEV) {
                    currentDefines[i] = buildType;
                }
            }
        } else {
            int lenght = currentDefines.Length;
            currentDefines = new string[lenght + 1];
            currentDefines[lenght] = buildType;
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
