namespace Beem.Extenject.Permissions {
    /// <summary>
    /// Microphone permission
    /// </summary>
    public interface IMicrophonePermission {
        string MicKey { get; }
        bool HasMicAccess { get; }
        void RequestMicAccess();
        bool MicRequestComplete { get; set; }
    }
}
