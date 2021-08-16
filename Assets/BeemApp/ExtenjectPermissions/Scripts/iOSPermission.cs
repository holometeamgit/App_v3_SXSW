using UnityEngine;

namespace Beem.Extenject.Permissions {
    /// <summary>
    /// Permission for iOS
    /// </summary>
    public class iOSPermission : ISettingsPermission {

        public void RequestSettings() {
#if UNITY_IOS && !UNITY_EDITOR
        string url = iOSSettingsOpenerBindings.GetSettingsURL();
        Application.OpenURL(url);
#endif
        }

    }
}
