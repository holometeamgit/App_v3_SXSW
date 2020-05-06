#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using UnityEngine;

public class iOSSettingsOpenerBindings : MonoBehaviour
{
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport ("__Internal")]
        public static extern string GetSettingsURL();

        [DllImport ("__Internal")]
        public static extern void OpenSettings();
#endif
}
