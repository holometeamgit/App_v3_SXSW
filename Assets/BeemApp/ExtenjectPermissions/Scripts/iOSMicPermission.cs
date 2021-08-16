using UnityEngine;

namespace Beem.Extenject.Permissions {
    /// <summary>
    /// Permission for iOS
    /// </summary>
    public class iOSMicPermission : IMicrophonePermission {

        public bool HasMicAccess => Application.HasUserAuthorization(UserAuthorization.Microphone);

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
            Application.RequestUserAuthorization(UserAuthorization.Microphone);
        }

    }
}
