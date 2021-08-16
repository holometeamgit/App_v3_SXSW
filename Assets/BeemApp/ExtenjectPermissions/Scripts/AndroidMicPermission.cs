using UnityEngine;
using UnityEngine.Android;

namespace Beem.Extenject.Permissions {

    /// <summary>
    /// Permission for Android
    /// </summary>
    public class AndroidMicPermission : IMicrophonePermission {

        public bool HasMicAccess => Permission.HasUserAuthorizedPermission(Permission.Microphone);

        public bool MicRequestComplete {
            get {
                return PlayerPrefs.GetString("Access for " + MicKey, "false") == "true";
            }
            set {
                PlayerPrefs.SetString("Access for " + MicKey, value ? "true" : "false");
            }
        }

        public string MicKey => "Microphone";

        public void RequestMicAccess() {
            Permission.RequestUserPermission(Permission.Microphone);
        }

    }
}
