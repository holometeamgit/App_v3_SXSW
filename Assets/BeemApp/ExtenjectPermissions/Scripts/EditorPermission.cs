using UnityEngine;

namespace Beem.Extenject.Permissions {
    /// <summary>
    /// Permission for Editor
    /// </summary>
    public class EditorPermission : ISettingsPermission {
        public void RequestSettings() {
            Debug.LogError($"{nameof(EditorPermission)} Requested Settings Editor");
        }
    }
}
