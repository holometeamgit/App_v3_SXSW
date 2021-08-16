using UnityEngine;

namespace Beem.Extenject.Permissions {
    /// <summary>
    /// Permission for Editor
    /// </summary>
    public class EditorMicPermission : IMicrophonePermission {

        public bool HasMicAccess => false;

        public string MicKey => "Microphone";
        public bool MicRequestComplete {
            get {
                return true;
            }
            set { }
        }

        public void RequestMicAccess() {
            Debug.LogError($"{nameof(EditorPermission)} Requested Mic Access Editor");
        }

    }
}
