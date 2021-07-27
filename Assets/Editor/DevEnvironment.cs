using System.Linq;
using UnityEditor;
using UnityEngine;

public class DevEnvironment : MonoBehaviour {
    static string Prod = "PROD";
    static string Dev = "DEV";

    [MenuItem("Environment/Switch To Dev")]
    static void SwitchToDev() {
        PlayerSettings.productName = "Beem Dev";

        SwitchDefine(BuildTargetGroup.iOS, Prod, Dev);
        SwitchDefine(BuildTargetGroup.Android, Prod, Dev);

        EditorUserBuildSettings.development = true;
    }

    [MenuItem("Environment/Switch To Prod")]
    static void SwitchToProd() {
        PlayerSettings.productName = "Beem";

        SwitchDefine(BuildTargetGroup.iOS, Dev, Prod);
        SwitchDefine(BuildTargetGroup.Android, Dev, Prod);

        EditorUserBuildSettings.development = false;
    }

    static void SwitchDefine(BuildTargetGroup targetGroup, string firstDefine, string secondDefine) {
        string[] currentDefines;
        PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out currentDefines);
        if (currentDefines.Contains(firstDefine)) {
            for (int i = 0; i < currentDefines.Length; i++) {
                if (currentDefines[i] == firstDefine) {
                    currentDefines[i] = secondDefine;
                }
            }
        } else {
            int lenght = currentDefines.Length;
            currentDefines = new string[lenght + 1];
            currentDefines[lenght] = secondDefine;
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
